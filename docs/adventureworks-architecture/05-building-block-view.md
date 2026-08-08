# 5. Building Block View

## 5.1 HumanResources Schema (6 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| department | departmentid, name, groupname | |
| employee | businessentityid, nationalidnumber, loginid, jobtitle, birthdate, maritalstatus, gender, hiredate, salariedflag, vacationhours, sickleavehours, currentflag, organizationnode | → person.person |
| employeedepartmenthistory | businessentityid, departmentid, shiftid, startdate, enddate | → employee, department, shift |
| employeepayhistory | businessentityid, ratechangedate, rate, payfrequency | → employee |
| jobcandidate | jobcandidateid, businessentityid, resume | → employee (nullable) |
| shift | shiftid, name, starttime, endtime | |

## 5.2 Person Schema (13 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| address | addressid, addressline1, addressline2, city, stateprovinceid, postalcode, spatiallocation | → stateprovince |
| addresstype | addresstypeid, name | |
| businessentity | businessentityid | |
| businessentityaddress | businessentityid, addressid, addresstypeid | → businessentity, address, addresstype |
| businessentitycontact | businessentityid, personid, contacttypeid | → businessentity, person, contacttype |
| contacttype | contacttypeid, name | |
| countryregion | countryregioncode, name | |
| emailaddress | businessentityid, emailaddressid, emailaddress | → person |
| password | businessentityid, passwordhash, passwordsalt | → person |
| person | businessentityid, persontype, namestyle, title, firstname, middlename, lastname, suffix, emailpromotion, additionalcontactinfo, demographics | → businessentity |
| personphone | businessentityid, phonenumber, phonenumbertypeid | → person, phonenumbertype |
| phonenumbertype | phonenumbertypeid, name | |
| stateprovince | stateprovinceid, stateprovincecode, countryregioncode, isonlystateprovinceflag, name, territoryid | → countryregion, sales.salesterritory |

## 5.3 Production Schema (25 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| billofmaterials | billofmaterialsid, productassemblyid, componentid, startdate, enddate, unitmeasurecode, bomlevel, perassemblyqty | → product (assembly & component), unitmeasure |
| culture | cultureid, name | |
| document | documentnode, title, owner, folderflag, filename, fileextension, revision, changenumber, status, documentsummary, document | → employee (owner) |
| illustration | illustrationid, diagram | |
| location | locationid, name, costrate, availability | |
| product | productid, name, productnumber, makeflag, finishedgoodsflag, color, safetystocklevel, reorderpoint, standardcost, listprice, size, sizeunitmeasurecode, weightunitmeasurecode, weight, weight_kg, daystomanufacture, productline, class, style, productsubcategoryid, productmodelid, sellstartdate, sellenddate, discontinueddate | → productsubcategory, productmodel, unitmeasure |
| productcategory | productcategoryid, name | |
| productcosthistory | productid, startdate, enddate, standardcost | → product |
| productdescription | productdescriptionid, description | |
| productdocument | productid, documentnode | → product, document |
| productinventory | productid, locationid, shelf, bin, quantity | → product, location |
| productlistpricehistory | productid, startdate, enddate, listprice | → product |
| productmodel | productmodelid, name, catalogdescription, instructions | |
| productmodelillustration | productmodelid, illustrationid | → productmodel, illustration |
| productmodelproductdescriptionculture | productmodelid, productdescriptionid, cultureid | → productmodel, productdescription, culture |
| productphoto | productphotoid, thumbnailphoto, thumbnailphotofilename, largephoto, largephotofilename | |
| productproductphoto | productid, productphotoid, primary | → product, productphoto |
| productreview | productreviewid, productid, reviewername, reviewdate, emailaddress, rating, comments | → product |
| productsubcategory | productsubcategoryid, productcategoryid, name | → productcategory |
| scrapreason | scrapreasonid, name | |
| transactionhistory | transactionid, productid, referenceorderid, referenceorderlineid, transactiondate, transactiontype, quantity, actualcost | → product |
| transactionhistoryarchive | transactionid, productid, referenceorderid, referenceorderlineid, transactiondate, transactiontype, quantity, actualcost | archived copy of transactionhistory |
| unitmeasure | unitmeasurecode, name | |
| workorder | workorderid, productid, orderqty, scrappedqty, startdate, enddate, duedate, scrapreasonid | → product, scrapreason |
| workorderrouting | workorderid, productid, operationsequence, locationid, scheduledstartdate, scheduledenddate, actualstartdate, actualenddate, actualresourcehrs, plannedcost, actualcost | → workorder, location |

## 5.4 Purchasing Schema (5 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| productvendor | productid, businessentityid, averageleadtime, standardprice, lastreceiptcost, lastreceiptdate, minorderqty, maxorderqty, onorderqty, unitmeasurecode | → production.product, vendor, production.unitmeasure |
| purchaseorderdetail | purchaseorderid, purchaseorderdetailid, duedate, orderqty, productid, unitprice, receivedqty, rejectedqty | → purchaseorderheader, production.product |
| purchaseorderheader | purchaseorderid, revisionnumber, status, employeeid, vendorid, shipmethodid, orderdate, shipdate, subtotal, taxamt, freight | → vendor, shipmethod, humanresources.employee |
| shipmethod | shipmethodid, name, shipbase, shiprate | |
| vendor | businessentityid, accountnumber, name, creditrating, preferredvendorstatus, activeflag, purchasingwebserviceurl | → person.businessentity |

## 5.5 Sales Schema (19 tables)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| countryregioncurrency | countryregioncode, currencycode | → person.countryregion, currency |
| creditcard | creditcardid, cardtype, cardnumber, expmonth, expyear | |
| currency | currencycode, name | |
| currencyrate | currencyrateid, currencyratedate, fromcurrencycode, tocurrencycode, averagerate, endofdayrate | → currency (from & to) |
| customer | customerid, personid, storeid, territoryid | → person.person, store, salesterritory |
| personcreditcard | businessentityid, creditcardid | → person.person, creditcard |
| salesorderdetail | salesorderid, salesorderdetailid, carriertrackingnumber, orderqty, productid, specialofferid, unitprice, unitpricediscount | → salesorderheader, production.product, specialofferproduct |
| salesorderheader | salesorderid, revisionnumber, orderdate, duedate, shipdate, status, onlineorderflag, purchaseordernumber, accountnumber, customerid, salespersonid, territoryid, billtoaddressid, shiptoaddressid, shipmethodid, creditcardid, creditcardapprovalcode, currencyrateid, subtotal, taxamt, freight, totaldue, comment | → customer, salesperson, salesterritory, person.address, purchasing.shipmethod, creditcard, currencyrate |
| salesorderheadersalesreason | salesorderid, salesreasonid | → salesorderheader, salesreason |
| salesperson | businessentityid, territoryid, salesquota, bonus, commissionpct, salesytd, saleslastyear | → humanresources.employee, salesterritory |
| salespersonquotahistory | businessentityid, quotadate, salesquota | → salesperson |
| salesreason | salesreasonid, name, reasontype | |
| salestaxrate | salestaxrateid, stateprovinceid, taxtype, taxrate, name | → person.stateprovince |
| salesterritory | territoryid, name, countryregioncode, group, salesytd, saleslastyear, costytd, costlastyear | → person.countryregion |
| salesterritoryhistory | businessentityid, territoryid, startdate, enddate | → salesperson, salesterritory |
| shoppingcartitem | shoppingcartitemid, shoppingcartid, quantity, productid, datecreated | → production.product |
| specialoffer | specialofferid, description, discountpct, type, category, startdate, enddate, minqty, maxqty | |
| specialofferproduct | specialofferid, productid | → specialoffer, production.product |
| store | businessentityid, name, salespersonid, demographics | → person.businessentity, salesperson |

## 5.6 Loyalty Schema (1 table)
| Table | Key Columns | Relations |
|-------|-------------|-----------|
| loyaltyaccount | loyaltyaccountid, customerid, points_balance, created_at, modified_at | → sales.customer |
