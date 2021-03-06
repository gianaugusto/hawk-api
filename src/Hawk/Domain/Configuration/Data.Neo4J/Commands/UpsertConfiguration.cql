﻿// (:Account)

MATCH 
  (account:Account)
WHERE
  account.email = $email

// (:Payee)

MATCH
  (payee:Payee)-[:BELONGS]->(account)
WHERE
  payee.name = $payee

// (:Category)

MATCH
  (category:Category)-[:BELONGS]->(account)
WHERE
  category.name = $category

// (:Currency)
  
MATCH 
  (currency:Currency)-[:BELONGS]->(account)
WHERE
  currency.name = $currency

// (:PaymentMethod)

MATCH 
  (paymentMethod:PaymentMethod)-[:BELONGS]->(account)
WHERE
  paymentMethod.name = $paymentMethod

// (:Configuration)
  
MERGE 
  (configuration:Configuration { 
    description: $description
  })
SET
  configuration.type = $type,
  configuration.description = $newDescription
MERGE
  (configuration)-[:BELONGS]->(account)
MERGE
  (configuration)-[:CONFIGURED_WITH]->(payee)
MERGE
  (configuration)-[:CONFIGURED_WITH]->(category)
MERGE
  (configuration)-[:CONFIGURED_WITH]->(currency)
MERGE
  (configuration)-[:CONFIGURED_WITH]->(paymentMethod)
WITH
  account,
  payee,
  category,
  currency,
  paymentMethod,
  configuration

// (:Tag)

UNWIND
  $tags AS tagName
MERGE
  (tag:Tag { 
    name: tagName
  })
MERGE
  (tag)-[:BELONGS]->(account)
MERGE
  (payee)-[:HAS]->(tag)
MERGE
  (configuration)-[:CONFIGURED_WITH]->(tag)
WITH
  account,
  configuration,
  payee,
  category,
  paymentMethod,
  currency,
  COLLECT(DISTINCT tag) AS tags

RETURN 
{ 
  description: configuration.description,
  type: configuration.type,
  account: {
    id: account.id,
	email: account.email
  },
  payee: CASE WHEN payee IS null THEN NULL ELSE {
    name: payee.name,
	transactions: SIZE((:Transaction)-[:IN]->(payee))
  } END,
  category: CASE WHEN category IS null THEN NULL ELSE {
    name: category.name,
	transactions: SIZE((:Transaction)-[:WITH]->(category))
  } END,
  currency: {
    name: currency.name,
    transactions: SIZE((:Transaction)-[:PAID_IN]->(currency))
  },
  paymentMethod: CASE WHEN paymentMethod IS null THEN NULL ELSE {
	name: paymentMethod.name,
    transactions: SIZE((:Transaction)-[:PAID_WITH]->(paymentMethod))
  } END,
  tags: [tag IN tags | {
    name: tag.name,
    transactions: SIZE((:Transaction)-[:HAS]->(tag))
  }]
} AS data;
