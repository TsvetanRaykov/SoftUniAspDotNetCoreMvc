# Project - Vertex Exchange Platform (VXP)

## Type - Brand relationship management 

## Description

A web platform to manage relationships between Vendor (brand), its partners (distributors) and the end users (customers).

### Administration

The platform will support the following roles: **Administrator**, **Vendor**, **Distributor** and **Customer**.

An administrator will be initially created.
Administrator can register users in all roles.
Vendor can only register distributors. 
Distributor can only generate distributor keys and provide them to its customers.
Customer can register himself via registration page only if got a distributor key. 

### Functionality

Customer can have more than one distributor. They can add a distributor by adding a distributor-key to their profile. Customer account can be active only if it relate to at least one distributor.
Customer can make orders to the distributors and add products to them. 
Customer can create projects and add orders to them. Initially created order has status **Draft**. 

Order must be a part of a project to become active.

Projects are process wrappers. They contains orders, related documents, and lists of messages (conversations) between the parties.

Document is just an uploaded file related to the project -  offers, receipts, invoices, etc.

Distributor must have a valid bank account. 
Distributor can receive orders only from his customers. He can accept or decline the order. 
Distributor can unregister himself from the customer list of distributors. 
Distributors can make orders only to the Vendor and add products to them.
Distributor can create projects and add orders, documents and messages to that projects.
Distributor can copy and modify copied orders from customer projects to vendor projects and vice versa. 

Vendor can receive orders only from distributors. 
Vendor can manage the status of received orders.
Vendor manage (CRUD) the products and their details. 

Product has dynamic set of details (dimensions, colors, prices etcetera specific attributes)

### Pricing model

Vendor set the base price of the product.
Vendor set the price modifiers (increase/decrease) to the distributors.

Distributor can set its price of the products.
Distributor can set the price modifiers (increase/decrease) to their customers. 

According to the range of influence, price modifiers are three types:

 - Global - applied to all products 
 - Category - applied to a specific product category
 - Product - applied to specific product

The smaller modifier range has a bigger priority.

## Entities

### User
  - Id (string)
  - Username (string)
  - FirstName (string)
  - LastName (string)
  - Password (string)
  - Email (string)
  - Address (Address)
  - Company (Company)
  - BankAccounts (list of BankAccount)
  - Distributors (list of Distributor Keys)
  - Projects (list of Project)
  - ReceivedMessages (list of MessageRecipient)
  -  PriceModifiersReceive (list of PriceModifier)
  - PriceModifiersGive (list of PriceModifier)
### Company

- Id (int)
- Name (string)
- BusinesNumber (string)
- ContactAddress (Address)
- ShippingAddress (Address)
- Members (list of User)

### Address

- Id (int)
- Country (Country)
- City (string)
- AddressLocation (string)
- Email (string)
- Phone (string)

### Country 

- Id (int)
- Name (string)
- Language (string)

### BankAccount

- Id (int)
- AccountNumber (string)
- BicCode (string)
- SwiftCode (string)
- BankName (string)
- Owner (User)

### DistributorKey

- Id (int)
- KeyCode (string)
- BankAccount (BankAccount)
- Customers (list of User)

### PriceModifier

- Id (int)
- Seller (User)
- Buyer (User)
- PriceModifierRange (Total, Category, Product)
- Name (string)
- Description (string)
- PriceModifierType (Decrease, Increase)
- PercentValue (decimal)

### ProductCategory

- Id (int)
- Name (string)

### Product

  - Id (int)
  - Name (string)
  - Description (string)
  - Category (ProductCategory)
  - Image (ProductImage )
  - Images (list of ProductImage)
  - IsAvailable (bool)
  - Details (list of ProductDetails)
### ProductDetail

- Id (int)
- Type (string) 
- Name (string)
- Value (string)
- Product (Product)

### Order
  - Id (int)
  - CreatedOn (DateTime)
  - Project (Project)
  - Products (list of OrderProduct)
  - Seller (User)
  - Buyer (User)
  - Deadline (DateTime)
  - Status (Enum: Draft, New, Pending, Accepted, Declined, InFactory, Ready, Dispatched, Delivered, Acquired)
### OrderProduct
  - Id (int)
  - Order (Order)
  - Product (Product)
  - Quantity (int)
### OrderHistory

- Id (int)
- Order (Order)
- OldStatus (Enum: Draft, New, Pending, ...)
- NewStatus (Enum: Draft, New, Pending, ...)

### ProductImage
  - Id (int)
  - URL (string)
  - Alt (string)
  - Title (string)
  - Product (Product)

### Document

- Id (int)
- Type  (Contract, Invoice, Offer, Other etc.)
- Description (string)
- Location (string)
- DocumentDate (DateTime)
- Project (Project)

### Project

- Id (int)
- Name (string)
- Description (string)
- Owner (User)

### Message 

- Id (int)
- Body (string)
- Topic (Message)
- Author (User)
- Recipients (list of MessageRecipient)

### MessageRecipient

- Message (Message) [key]

- Recipient (User) [key]

- ReadOn (DateTime)

  

[VXP Project Repository](https://github.com/TsvetanRaykov/SoftUniAspDotNetCoreMvc "Github repository of the project")

[VXP Database Model](https://github.com/TsvetanRaykov/SoftUniAspDotNetCoreMvc/blob/master/VxpDatabase.png?raw=true "Database diagram 4589x3089")





![VXP Database Model](https://github.com/TsvetanRaykov/SoftUniAspDotNetCoreMvc/blob/master/VxpDatabase.png?raw=true)

