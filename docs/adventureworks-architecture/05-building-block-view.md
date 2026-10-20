# 5. Building Block View

## 5.1 HumanResources Schema (6 tables)

### department
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| departmentid | integer | NO |
| name | character varying | NO |
| groupname | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### employee
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| nationalidnumber | character varying | NO |
| loginid | character varying | NO |
| jobtitle | character varying | NO |
| birthdate | date | NO |
| maritalstatus | character | NO |
| gender | character | NO |
| hiredate | date | NO |
| salariedflag | boolean | NO |
| vacationhours | smallint | NO |
| sickleavehours | smallint | NO |
| currentflag | boolean | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |
| organizationnode | character varying | YES |
| currentpayrate | numeric | YES |
| currentpayfrequency | smallint | YES |
| paychangedate | timestamp without time zone | YES |

### employeedepartmenthistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| departmentid | smallint | NO |
| shiftid | smallint | NO |
| startdate | date | NO |
| enddate | date | YES |
| modifieddate | timestamp without time zone | NO |

### employeepayhistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| ratechangedate | timestamp without time zone | NO |
| rate | numeric | NO |
| payfrequency | smallint | NO |
| modifieddate | timestamp without time zone | NO |

### jobcandidate
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| jobcandidateid | integer | NO |
| businessentityid | integer | YES |
| resume | xml | YES |
| modifieddate | timestamp without time zone | NO |

### shift
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| shiftid | integer | NO |
| name | character varying | NO |
| starttime | time without time zone | NO |
| endtime | time without time zone | NO |
| modifieddate | timestamp without time zone | NO |


## 5.2 Person Schema (13 tables)

### address
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| addressid | integer | NO |
| addressline1 | character varying | NO |
| addressline2 | character varying | YES |
| city | character varying | NO |
| stateprovinceid | integer | NO |
| postalcode | character varying | NO |
| spatiallocation | bytea | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### addresstype
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| addresstypeid | integer | NO |
| name | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### businessentity
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### businessentityaddress
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| addressid | integer | NO |
| addresstypeid | integer | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### businessentitycontact
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| personid | integer | NO |
| contacttypeid | integer | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### contacttype
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| contacttypeid | integer | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### countryregion
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| countryregioncode | character varying | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### emailaddress
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| emailaddressid | integer | NO |
| emailaddress | character varying | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### password
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| passwordhash | character varying | NO |
| passwordsalt | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### person
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| persontype | character | NO |
| namestyle | boolean | NO |
| title | character varying | YES |
| firstname | character varying | NO |
| middlename | character varying | YES |
| lastname | character varying | NO |
| suffix | character varying | YES |
| emailpromotion | integer | NO |
| additionalcontactinfo | xml | YES |
| demographics | xml | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### personphone
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| phonenumber | character varying | NO |
| phonenumbertypeid | integer | NO |
| modifieddate | timestamp without time zone | NO |

### phonenumbertype
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| phonenumbertypeid | integer | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### stateprovince
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| stateprovinceid | integer | NO |
| stateprovincecode | character | NO |
| countryregioncode | character varying | NO |
| isonlystateprovinceflag | boolean | NO |
| name | character varying | NO |
| territoryid | integer | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |


## 5.3 Production Schema (25 tables)

### billofmaterials
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| billofmaterialsid | integer | NO |
| productassemblyid | integer | YES |
| componentid | integer | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | YES |
| unitmeasurecode | character | NO |
| bomlevel | smallint | NO |
| perassemblyqty | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### culture
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| cultureid | character | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### document
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| title | character varying | NO |
| owner | integer | NO |
| folderflag | boolean | NO |
| filename | character varying | NO |
| fileextension | character varying | YES |
| revision | character | NO |
| changenumber | integer | NO |
| status | smallint | NO |
| documentsummary | text | YES |
| document | bytea | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |
| documentnode | character varying | NO |

### illustration
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| illustrationid | integer | NO |
| diagram | xml | YES |
| modifieddate | timestamp without time zone | NO |

### location
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| locationid | integer | NO |
| name | character varying | NO |
| costrate | numeric | NO |
| availability | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### product
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| name | character varying | NO |
| productnumber | character varying | NO |
| makeflag | boolean | NO |
| finishedgoodsflag | boolean | NO |
| color | character varying | YES |
| safetystocklevel | smallint | NO |
| reorderpoint | smallint | NO |
| standardcost | numeric | NO |
| listprice | numeric | NO |
| size | character varying | YES |
| sizeunitmeasurecode | character | YES |
| weightunitmeasurecode | character | YES |
| weight | numeric | YES |
| daystomanufacture | integer | NO |
| productline | character | YES |
| class | character | YES |
| style | character | YES |
| productsubcategoryid | integer | YES |
| productmodelid | integer | YES |
| sellstartdate | timestamp without time zone | NO |
| sellenddate | timestamp without time zone | YES |
| discontinueddate | timestamp without time zone | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |
| weight_kg | numeric | YES |

### productcategory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productcategoryid | integer | NO |
| name | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### productcosthistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | YES |
| standardcost | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### productdescription
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productdescriptionid | integer | NO |
| description | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### productdocument
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| modifieddate | timestamp without time zone | NO |
| documentnode | character varying | NO |

### productinventory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| locationid | smallint | NO |
| shelf | character varying | NO |
| bin | smallint | NO |
| quantity | smallint | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### productlistpricehistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | YES |
| listprice | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### productmodel
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productmodelid | integer | NO |
| name | character varying | NO |
| catalogdescription | xml | YES |
| instructions | xml | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |


### productmodelillustration
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productmodelid | integer | NO |
| illustrationid | integer | NO |
| modifieddate | timestamp without time zone | NO |

### productmodelproductdescriptionculture
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productmodelid | integer | NO |
| productdescriptionid | integer | NO |
| cultureid | character | NO |
| modifieddate | timestamp without time zone | NO |

### productphoto
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productphotoid | integer | NO |
| thumbnailphoto | bytea | YES |
| thumbnailphotofilename | character varying | YES |
| largephoto | bytea | YES |
| largephotofilename | character varying | YES |
| modifieddate | timestamp without time zone | NO |

### productproductphoto
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| productphotoid | integer | NO |
| primary | boolean | NO |
| modifieddate | timestamp without time zone | NO |

### productreview
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productreviewid | integer | NO |
| productid | integer | NO |
| reviewername | character varying | NO |
| reviewdate | timestamp without time zone | NO |
| emailaddress | character varying | NO |
| rating | integer | NO |
| comments | character varying | YES |
| modifieddate | timestamp without time zone | NO |

### productsubcategory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productsubcategoryid | integer | NO |
| productcategoryid | integer | NO |
| name | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### scrapreason
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| scrapreasonid | integer | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### transactionhistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| transactionid | integer | NO |
| productid | integer | NO |
| referenceorderid | integer | NO |
| referenceorderlineid | integer | NO |
| transactiondate | timestamp without time zone | NO |
| transactiontype | character | NO |
| quantity | integer | NO |
| actualcost | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### transactionhistoryarchive
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| transactionid | integer | NO |
| productid | integer | NO |
| referenceorderid | integer | NO |
| referenceorderlineid | integer | NO |
| transactiondate | timestamp without time zone | NO |
| transactiontype | character | NO |
| quantity | integer | NO |
| actualcost | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### unitmeasure
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| unitmeasurecode | character | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### workorder
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| workorderid | integer | NO |
| productid | integer | NO |
| orderqty | integer | NO |
| scrappedqty | smallint | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | YES |
| duedate | timestamp without time zone | NO |
| scrapreasonid | smallint | YES |
| modifieddate | timestamp without time zone | NO |

### workorderrouting
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| workorderid | integer | NO |
| productid | integer | NO |
| operationsequence | smallint | NO |
| locationid | smallint | NO |
| scheduledstartdate | timestamp without time zone | NO |
| scheduledenddate | timestamp without time zone | NO |
| actualstartdate | timestamp without time zone | YES |
| actualenddate | timestamp without time zone | YES |
| actualresourcehrs | numeric | YES |
| plannedcost | numeric | NO |
| actualcost | numeric | YES |
| modifieddate | timestamp without time zone | NO |


## 5.4 Purchasing Schema (7 tables)

### productvendor
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| productid | integer | NO |
| businessentityid | integer | NO |
| averageleadtime | integer | NO |
| standardprice | numeric | NO |
| lastreceiptcost | numeric | YES |
| lastreceiptdate | timestamp without time zone | YES |
| minorderqty | integer | NO |
| maxorderqty | integer | NO |
| onorderqty | integer | YES |
| unitmeasurecode | character | NO |
| modifieddate | timestamp without time zone | NO |

### purchaseorderdetail
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| purchaseorderid | integer | NO |
| purchaseorderdetailid | integer | NO |
| duedate | timestamp without time zone | NO |
| orderqty | smallint | NO |
| productid | integer | NO |
| unitprice | numeric | NO |
| receivedqty | numeric | NO |
| rejectedqty | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### purchaseorderheader
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| purchaseorderid | integer | NO |
| revisionnumber | smallint | NO |
| status | smallint | NO |
| employeeid | integer | NO |
| vendorid | integer | NO |
| shipmethodid | integer | NO |
| orderdate | timestamp without time zone | NO |
| shipdate | timestamp without time zone | YES |
| subtotal | numeric | NO |
| taxamt | numeric | NO |
| freight | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### purchaserequest
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| purchaserequestid | integer | NO |
| employeeid | integer | NO |
| productid | integer | NO |
| quantity | smallint | NO |
| requestdate | timestamp without time zone | NO |
| status | character varying | NO |

### purchasereturn
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| purchasereturnid | integer | NO |
| purchaseorderdetailid | integer | NO |
| returndate | timestamp without time zone | NO |
| quantity | smallint | NO |
| reason | character varying | YES |
| modifieddate | timestamp without time zone | NO |

### shipmethod
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| shipmethodid | integer | NO |
| name | character varying | NO |
| shipbase | numeric | NO |
| shiprate | numeric | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### vendor
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| accountnumber | character varying | NO |
| name | character varying | NO |
| creditrating | smallint | NO |
| preferredvendorstatus | boolean | NO |
| activeflag | boolean | NO |
| purchasingwebserviceurl | character varying | YES |
| modifieddate | timestamp without time zone | NO |


## 5.5 Sales Schema (19 tables)

### countryregioncurrency
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| countryregioncode | character varying | NO |
| currencycode | character | NO |
| modifieddate | timestamp without time zone | NO |

### creditcard
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| creditcardid | integer | NO |
| cardtype | character varying | NO |
| cardnumber | character varying | NO |
| expmonth | smallint | NO |
| expyear | smallint | NO |
| modifieddate | timestamp without time zone | NO |

### currency
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| currencycode | character | NO |
| name | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### currencyrate
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| currencyrateid | integer | NO |
| currencyratedate | timestamp without time zone | NO |
| fromcurrencycode | character | NO |
| tocurrencycode | character | NO |
| averagerate | numeric | NO |
| endofdayrate | numeric | NO |
| modifieddate | timestamp without time zone | NO |

### customer
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| customerid | integer | NO |
| personid | integer | YES |
| storeid | integer | YES |
| territoryid | integer | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### personcreditcard
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| creditcardid | integer | NO |
| modifieddate | timestamp without time zone | NO |

### salesorderdetail
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| salesorderid | integer | NO |
| salesorderdetailid | integer | NO |
| carriertrackingnumber | character varying | YES |
| orderqty | smallint | NO |
| productid | integer | NO |
| specialofferid | integer | NO |
| unitprice | numeric | NO |
| unitpricediscount | numeric | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salesorderheader
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| salesorderid | integer | NO |
| revisionnumber | smallint | NO |
| orderdate | timestamp without time zone | NO |
| duedate | timestamp without time zone | NO |
| shipdate | timestamp without time zone | YES |
| status | smallint | NO |
| onlineorderflag | boolean | NO |
| purchaseordernumber | character varying | YES |
| accountnumber | character varying | YES |
| customerid | integer | NO |
| salespersonid | integer | YES |
| territoryid | integer | YES |
| billtoaddressid | integer | NO |
| shiptoaddressid | integer | NO |
| shipmethodid | integer | NO |
| creditcardid | integer | YES |
| creditcardapprovalcode | character varying | YES |
| currencyrateid | integer | YES |
| subtotal | numeric | NO |
| taxamt | numeric | NO |
| freight | numeric | NO |
| totaldue | numeric | YES |
| comment | character varying | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salesorderheadersalesreason
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| salesorderid | integer | NO |
| salesreasonid | integer | NO |
| modifieddate | timestamp without time zone | NO |


### salesperson
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| territoryid | integer | YES |
| salesquota | numeric | YES |
| bonus | numeric | NO |
| commissionpct | numeric | NO |
| salesytd | numeric | NO |
| saleslastyear | numeric | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salespersonquotahistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| quotadate | timestamp without time zone | NO |
| salesquota | numeric | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salesreason
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| salesreasonid | integer | NO |
| name | character varying | NO |
| reasontype | character varying | NO |
| modifieddate | timestamp without time zone | NO |

### salestaxrate
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| salestaxrateid | integer | NO |
| stateprovinceid | integer | NO |
| taxtype | smallint | NO |
| taxrate | numeric | NO |
| name | character varying | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salesterritory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| territoryid | integer | NO |
| name | character varying | NO |
| countryregioncode | character varying | NO |
| group | character varying | NO |
| salesytd | numeric | NO |
| saleslastyear | numeric | NO |
| costytd | numeric | NO |
| costlastyear | numeric | NO |
| manager_id | integer | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### salesterritoryhistory
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| territoryid | integer | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### shoppingcartitem
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| shoppingcartitemid | integer | NO |
| shoppingcartid | character varying | NO |
| quantity | integer | NO |
| productid | integer | NO |
| datecreated | timestamp without time zone | NO |
| modifieddate | timestamp without time zone | NO |

### specialoffer
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| specialofferid | integer | NO |
| description | character varying | NO |
| discountpct | numeric | NO |
| type | character varying | NO |
| category | character varying | NO |
| startdate | timestamp without time zone | NO |
| enddate | timestamp without time zone | NO |
| minqty | integer | NO |
| maxqty | integer | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |
| discount_percentage | numeric | YES |

### specialofferproduct
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| specialofferid | integer | NO |
| productid | integer | NO |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

### store
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| businessentityid | integer | NO |
| name | character varying | NO |
| salespersonid | integer | YES |
| demographics | xml | YES |
| rowguid | uuid | NO |
| modifieddate | timestamp without time zone | NO |

## 5.6 Loyalty Schema (1 table)

### loyaltyaccount
| Column Name | Data Type | Nullable |
|-------------|-----------|----------|
| loyaltyaccountid | integer | NO |
| customerid | integer | NO |
| points_balance | integer | NO |
| created_at | timestamp without time zone | NO |
| modified_at | timestamp without time zone | NO |
| tier_expiry_date | timestamp without time zone | YES |
