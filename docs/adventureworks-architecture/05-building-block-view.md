# 5. Building Block View

## 5.1 HumanResources Schema (6 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| employee | businessentityid, jobtitle, hiredate | → person.person |
| department | departmentid, name, groupname | |
| employeedepartmenthistory | businessentityid, departmentid, startdate | → employee, department |
| employeepayhistory | businessentityid, ratechangedate, rate | → employee |
| jobcandidate | jobcandidateid, businessentityid | |
| shift | shiftid, name, starttime, endtime | |

## 5.2 Person Schema (9 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| person | businessentityid, firstname, lastname, emailpromotion | → businessentity |
| businessentity | businessentityid | |
| address | addressid, addressline1, city, postalcode | → stateprovince |
| stateprovince | stateprovinceid, countryregioncode | → countryregion |
| emailaddress | businessentityid, emailaddress | → person |
| personphone | businessentityid, phonenumber | → person |

## 5.3 Production Schema (25 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| product | productid, name, productnumber, standardcost, listprice | → productsubcategory, productmodel |
| productcategory | productcategoryid, name | |
| productsubcategory | productsubcategoryid, productcategoryid, name | → productcategory |
| productinventory | productid, locationid, quantity | → product, location |
| billofmaterials | billofmaterialsid, productassemblyid, componentid | → product |
| transactionhistory | transactionid, productid, transactiontype, quantity | → product |

## 5.4 Purchasing Schema (3 tables — intentionally sparse)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| vendor | businessentityid, accountnumber, name, creditrating | |
| shipmethod | shipmethodid, name, shipbase, shiprate | |
| purchaseorderheader | purchaseorderid, vendorid, orderdate, totaldue | → vendor, shipmethod |

⚠️ Note: purchaseorderdetail table is missing — this is Scenario 1 for
DB Agent evaluation.

## 5.5 Sales Schema (25 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| customer | customerid, personid, storeid, territoryid | → person, store, salesterritory |
| salesorderheader | salesorderid, customerid, orderdate, totaldue | → customer, salesterritory |
| salesorderdetail | salesorderdetailid, salesorderid, productid, unitprice | → salesorderheader, product |
| salesterritory | territoryid, name, countryregioncode | |
| currency | currencycode, name | |
| currencyrate | currencyrateid, currencycode, averagerate | → currency |
| store | businessentityid, name | |
| specialoffer | specialofferid, description, discountpct | |
