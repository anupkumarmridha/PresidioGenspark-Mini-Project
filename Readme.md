                     
<h1 align="center" style="font-weight: bold;">APIApparel Shopping App Project Requirements</h1>


<p align="center">Here is a detailed list of requirements for the APIApparel Shopping App project, including the necessary models and filters for documentation

### 1. **Authentication & Authorization**
   - **User Registration**
     - **Customer Registration** 
         - Endpoint: `POST /api/User/CustomerRegister`
         - Request Body: `{ "username": "string", "email": "string", "phone": "string" "password": "string" }`
         - Response: `{ "message": "User registered successfully", "userId": "int" }`
      
     - **Seller Registration** 
         - Endpoint: `POST /api/User/SellerRegister`
         - Request Body: `{ "username": "string", "email": "string", "phone": "string" "password": "string" }`
         - Response: `{ "message": "User registered successfully", "userId": "int" }`

   - **User Login**
     - **Customer Login** 
         - Endpoint: `POST /api/User/CustomerLogin`
         - Request Body: `{ "email": "string", "password": "string" }`
         - Response: `{ "token": "string", "userId": "int", "Role": "string" }`
      
     - **Seller Login** 
         - Endpoint: `POST /api/User/SellerLogin`
         - Request Body: `{ "email": "string", "password": "string" }`
         - Response: `{ "token": "string", "userId": "int", "Role": "string"}`

### 2. **User Management**
   - **User Model**
     - Properties: `PK->UserId`, `Hash Password`, `password`, `Role` (Customer/Seller/Admin)
   - **Customer Model**
     - Properties: `PK->CustomerId(FK->UserId)` , `Email`, `FullName`, `PhoneNumber`
   - **Seller Model**
     - Properties: `PK->SellerId(FK->UserId)`, `Email`, `FullName`, `PhoneNumber`

### 3. **Address Management**
   - **Address Model**
     - Properties: `AddressId`, `UserId`, `Street`, `City`, `State`, `PostalCode`, `Country`
   - **CRUD Operations for Address**
     - Add, Update, Delete, Retrieve addresses for a user

### 4. **Product Management**
   - **Product Model**
     - Properties: `ProductId`, `Name`, `Description`, `Price`, `Quantity`, `CategoryId`, `SellerId`, `ImageUrl`, `CreationDate`, `LastUpdatedDate`
   - **Category Model**
     - Properties: `CategoryId`, `Name`
   - **Product CRUD Operations**
     - Add, Update, Delete, Retrieve products
   - **Product Filters**
     - By Category, Price Range, Availability, Rating, Seller

### 5. **Order Management**
   - **Order Model**
     - Properties: `OrderId`, `CustomerId`, `OrderDate`, `Status`, `TotalAmount`, `PaymentId`, `ShippingAddressId`
   - **OrderDetails Model**
     - Properties: `OrderDetailsId`, `OrderId`, `ProductId`, `Quantity`, `Price`
   - **CRUD Operations for Orders**
     - Place, Update, Cancel, Retrieve orders for a customer

### 6. **Payment Management**
   - **Payment Model**
     - Properties: `PaymentId`, `OrderId`, `Amount`, `PaymentMethod`, `PaymentDate`, `Status`
   - **Payment Processing**
     - Capture payment details during order placement

### 7. **Cart Management**
   - **Cart Model**
     - Properties: `CartId`, `CustomerId`
   - **CartItem Model**
     - Properties: `CartItemId`, `CartId`, `ProductId`, `Quantity`
   - **CRUD Operations for Cart**
     - Add, Update, Remove items from the cart

### 8. **Review Management**
   - **Review Model**
     - Properties: `ReviewId`, `ProductId`, `CustomerId`, `Rating`, `Comment`, `ReviewDate`
   - **CRUD Operations for Reviews**
     - Add, Update, Delete, Retrieve reviews for a product

### API Endpoints Summary

#### Authentication
   -  `POST /api/User/CustomerRegister`
   -  `POST /api/User/SellerRegister`
   -  `POST /api/User/CustomerLogin`
   - `POST /api/User/SellerLogin`

#### Users
   - `GET /api/users/{id}`
   - `PUT /api/users/{id}`
   - `DELETE /api/users/{id}`

#### Addresses
   - `GET /api/addresses`
   - `POST /api/addresses`
   - `PUT /api/addresses/{id}`
   - `DELETE /api/addresses/{id}`

#### Products
   - `GET /api/products`
     - Filters: `category`, `priceRange`, `availability`, `rating`, `seller`
   - `POST /api/products`
   - `PUT /api/products/{id}`
   - `DELETE /api/products/{id}`
   - `GET /api/products/{id}`

#### Categories
   - `GET /api/categories`
   - `POST /api/categories`
   - `PUT /api/categories/{id}`
   - `DELETE /api/categories/{id}`

#### Orders
   - `GET /api/orders`
   - `POST /api/orders`
   - `PUT /api/orders/{id}`
   - `DELETE /api/orders/{id}`
   - `GET /api/orders/{id}`

#### Payments
   - `GET /api/payments`
   - `POST /api/payments`
   - `PUT /api/payments/{id}`
   - `DELETE /api/payments/{id}`
   - `GET /api/payments/{id}`

#### Cart
   - `GET /api/cart`
   - `POST /api/cart/items`
   - `PUT /api/cart/items/{id}`
   - `DELETE /api/cart/items/{id}`

#### Reviews
   - `GET /api/reviews`
   - `POST /api/reviews`
   - `PUT /api/reviews/{id}`
   - `DELETE /api/reviews/{id}`
   - `GET /api/reviews/{id}`

### Filters and Sorting

1. **Product Filters**
   - **By Category**
     - Endpoint: `GET /api/products?category={categoryId}`
   - **By Price Range**
     - Endpoint: `GET /api/products?minPrice={min}&maxPrice={max}`
   - **By Availability**
     - Endpoint: `GET /api/products?availability={true/false}`
   - **By Rating**
     - Endpoint: `GET /api/products?minRating={rating}`
   - **By Seller**
     - Endpoint: `GET /api/products?sellerId={sellerId}`

2. **Order Filters**
   - **By Status**
     - Endpoint: `GET /api/orders?status={status}`
   - **By Date Range**
     - Endpoint: `GET /api/orders?startDate={start}&endDate={end}`

3. **Review Filters**
   - **By Rating**
     - Endpoint: `GET /api/reviews?minRating={rating}`
   - **By Product**
     - Endpoint: `GET /api/reviews?productId={productId}`

This provides a comprehensive view of the API endpoints and models needed for the Apparel Shopping App project, including necessary CRUD operations and filters for effective data retrieval and management.</p>


