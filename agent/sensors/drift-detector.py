#!/usr/bin/env python3
"""
Schema Drift Detector — Agent Sensor
Compares arc42 architecture documentation with actual PostgreSQL database state.
Reports tables/columns present in DB but not documented, and vice versa.
"""

import re
import sys
from datetime import datetime

try:
    import psycopg2
except ImportError:
    print("Installing psycopg2...")
    import subprocess
    subprocess.run([sys.executable, "-m", "pip", "install", "psycopg2-binary", "--break-system-packages"], check=True)
    import psycopg2

# ─── Configuration ────────────────────────────────────────────────────────────

DB_CONFIG = {
    "host": "localhost",
    "port": 5433,
    "database": "Adventureworks",
    "user": "postgres",
    "password": "postgres"
}

ARC42_PATH = "docs/adventureworks-architecture/05-building-block-view.md"

SCHEMAS_TO_CHECK = [
    "humanresources",
    "person",
    "production",
    "purchasing",
    "sales",
    "loyalty"
]

# ─── Parse arc42 Chapter 5 ────────────────────────────────────────────────────

def parse_arc42(filepath):
    """
    Parse arc42 Chapter 5 (Building Block View) and extract
    documented tables per schema.
    Returns: dict { schema: [table1, table2, ...] }
    """
    documented = {}

    try:
        with open(filepath, "r", encoding="utf-8") as f:
            content = f.read()
    except FileNotFoundError:
        print(f"ERROR: arc42 file not found at {filepath}")
        print("       Run from repo root directory.")
        sys.exit(1)

    # Find schema sections (## 5.x <Schema> Schema)
    section_pattern = re.compile(
        r"## 5\.\d+ (\w+) Schema[^\n]*\n(.*?)(?=## |\Z)",
        re.DOTALL | re.IGNORECASE
    )

    # Find table names in markdown tables (| tablename | ... |)
    table_pattern = re.compile(r"^\|\s*(\w+)\s*\|", re.MULTILINE)

    for match in section_pattern.finditer(content):
        schema_name = match.group(1).lower()
        section_content = match.group(2)

        tables = []
        for table_match in table_pattern.finditer(section_content):
            table_name = table_match.group(1).lower()
            # Skip header rows
            if table_name not in ("table", "name", "schema", "context", "key"):
                tables.append(table_name)

        if tables:
            documented[schema_name] = tables

    return documented


# ─── Query actual database state ──────────────────────────────────────────────

def get_db_state():
    """
    Query information_schema for actual tables and columns in the database.
    Returns: dict { schema: { table: [col1, col2, ...] } }
    """
    db_state = {}

    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # Get all tables per schema
        cursor.execute("""
            SELECT table_schema, table_name
            FROM information_schema.tables
            WHERE table_schema = ANY(%s)
              AND table_type = 'BASE TABLE'
            ORDER BY table_schema, table_name;
        """, (SCHEMAS_TO_CHECK,))

        for schema, table in cursor.fetchall():
            if schema not in db_state:
                db_state[schema] = {}
            db_state[schema][table] = []

        # Get all columns per table
        cursor.execute("""
            SELECT table_schema, table_name, column_name, data_type, is_nullable
            FROM information_schema.columns
            WHERE table_schema = ANY(%s)
            ORDER BY table_schema, table_name, ordinal_position;
        """, (SCHEMAS_TO_CHECK,))

        for schema, table, column, dtype, nullable in cursor.fetchall():
            if schema in db_state and table in db_state[schema]:
                db_state[schema][table].append({
                    "name": column,
                    "type": dtype,
                    "nullable": nullable
                })

        cursor.close()
        conn.close()

    except psycopg2.OperationalError as e:
        print(f"ERROR: Cannot connect to database: {e}")
        print("       Make sure AdventureWorks Docker container is running on port 5433.")
        sys.exit(1)

    return db_state


# ─── Compare and generate report ──────────────────────────────────────────────

def generate_report(documented, db_state):
    """
    Compare documented state (arc42) with actual DB state.
    Returns drift report as string.
    """
    lines = []
    lines.append("=" * 65)
    lines.append("SCHEMA DRIFT REPORT")
    lines.append(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    lines.append(f"arc42 source: {ARC42_PATH}")
    lines.append("=" * 65)
    lines.append("")

    total_drift = 0
    total_ok = 0

    # Check all schemas in database
    all_schemas = set(list(documented.keys()) + list(db_state.keys()))

    for schema in sorted(all_schemas):
        lines.append(f"SCHEMA: {schema}")
        lines.append("-" * 40)

        doc_tables = set(documented.get(schema, []))
        db_tables = set(db_state.get(schema, {}).keys())

        # Schema in DB but not documented
        if schema in db_state and schema not in documented:
            lines.append(f"  ⚠️  DRIFT: Schema exists in database but NOT documented in arc42")
            total_drift += 1

        # Schema documented but not in DB
        elif schema in documented and schema not in db_state:
            lines.append(f"  ⚠️  DRIFT: Schema documented in arc42 but NOT found in database")
            total_drift += 1

        else:
            # Tables in DB but not documented
            undocumented_tables = db_tables - doc_tables
            missing_tables = doc_tables - db_tables

            if not undocumented_tables and not missing_tables:
                lines.append(f"  ✅ OK — all documented tables present")
                total_ok += 1
            else:
                if undocumented_tables:
                    for t in sorted(undocumented_tables):
                        cols = db_state[schema].get(t, [])
                        col_names = ", ".join(c["name"] for c in cols[:5])
                        if len(cols) > 5:
                            col_names += f" ... (+{len(cols)-5} more)"
                        lines.append(f"  ⚠️  DRIFT: '{t}' exists in DB but NOT in arc42")
                        lines.append(f"       Columns: {col_names}")
                        total_drift += 1

                if missing_tables:
                    for t in sorted(missing_tables):
                        lines.append(f"  ❌ MISSING: '{t}' documented in arc42 but NOT in database")
                        total_drift += 1

        lines.append("")

    # Summary
    lines.append("=" * 65)
    lines.append("SUMMARY")
    lines.append("-" * 40)
    lines.append(f"  Schemas checked:     {len(all_schemas)}")
    lines.append(f"  OK (no drift):       {total_ok}")
    lines.append(f"  Drift detected:      {total_drift}")
    lines.append("")

    if total_drift == 0:
        lines.append("  ✅ No drift detected — database matches arc42 documentation.")
    else:
        lines.append(f"  ⚠️  {total_drift} drift(s) detected.")
        lines.append("     Consider updating arc42 Ch.5 or creating migrations.")
        lines.append("     Run Architecture agent to update documentation.")

    lines.append("=" * 65)

    return "\n".join(lines)


# ─── Main ─────────────────────────────────────────────────────────────────────

def main():
    print("Schema Drift Detector — reading arc42 documentation...")
    documented = parse_arc42(ARC42_PATH)
    print(f"  Documented schemas: {list(documented.keys())}")

    print("Querying database state...")
    db_state = get_db_state()
    print(f"  Database schemas: {list(db_state.keys())}")

    print("Generating drift report...\n")
    report = generate_report(documented, db_state)
    print(report)

    # Save report to file
    report_path = f"agent/sensors/drift-report-{datetime.now().strftime('%Y%m%d-%H%M%S')}.txt"
    with open(report_path, "w", encoding="utf-8") as f:
        f.write(report)
    print(f"\nReport saved to: {report_path}")

    # Exit with error code if drift detected
    if "drift(s) detected" in report:
        sys.exit(1)
    sys.exit(0)


if __name__ == "__main__":
    main()
