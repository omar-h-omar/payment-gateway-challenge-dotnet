# Payment Gateway

***This documentation was last updated on 10/02/2025.***

An API gateway that will allow a merchant to offer a way for their shoppers to pay for their product. This service provides 2 payment endpoints to process a payment and retrive a previous payment.

**Processing a Payment**
```mermaid
graph LR;
    A[Shopper]<-->B[Merchant];
    B[Merchant]<-->C[Payment Gateway];
    C[Payment Gateway]<-->D[Acquiring Bank];
```

## Assumptions
* I've assumed that the requirement to return a Rejected response to the merchant when a request has invalid information means to return a BadRequest response instead of returning a payment response with the status Rejected.

## Design and Rationale
The service is divided into separate folders for Controllers, Services, Repositories, HttpClients, and Models. Separating the folders ensures we maintain a clean hierarchy and makes it easy to traverse the service. The folders contain the following:

### Controllers
#### Payments Controller
Contains the 2 main endpoints for our application and is responsible for making sure that the requests are validated and that correct status codes and errors are returned. The controller holds no logic around how to process or get a payment. Instead, it relies on the payments service to handle these operations. Similarly it relies on validators to validate requests.

### Services
#### Payments Service
Contains the logic to process and get a payment. By separating the logic from the controller, we are able to reuse these operation in other areas of our application like other controllers or subscribers without duplicating the code. We're also abstracting any future updates to the logic since the different controllers only call the interface function. In terms of testing, we are able to test this logic in isolation without worrying about the different response codes that the controller needs to return.

### Repositories
#### Payments Repository
Contains the logic to save and load a payment. The implementation here was not changed as it was not part of the requirements. However, an interface was added to make it easier to swap out the implementation with a mock during testing.

### HttpClients
#### Acquiring Bank Client
Contains the logic to send http requests to the acquiring bank. It also deseralises the http responses and ensures the response code is a successful one.

### Models
Contains all the models and validators for controller/http client requests and responses. Some of the models were modified to meet the requirements set out by the brief.

#### Post Payment Request Validator
Validate the request properties against the requirements set out in the brief. The validation is powered by the fluentvalidation package which comes built with a lot of functionality. Each property has a validation rule defined for it and some rules are dependant on others passing before they can run.

## Testing Approach
The tests used in the service are divided into 2 projects one for integration and another unit tests. The aim is to combine both tests to ensure our service is working correctly at a high level and a low level.

The integration tests aim to verify how the service components work together by running tests that encompass the different levels within the service and mocking any outside dependencies such as the acquiring bank. In our case the integration tests are verifying the different controller behaviour scenarios. Wiremock was used to replicate the behviour of the acquiring bank.

The unit tests aim to verify that each specific component within the service works as expected independently of other components. Tests were added for Controllers, Services, Repositories, HttpClients and Validators.

## Future Improvements
In terms of extending this service for real world use, I think the following improvements should be considered/implemented:

* The service should be put in a container to ensure our service can run on different systems within a consistent environment and avoid manual environment configrations. We should also include any deployment configuration like Helm charts.

* The repository layer should be replaced with an actual database integration and we should create separate entity objects for the database. This avoids linking between requests classes and database entities giving us more control when we want to modify our requests without impacting our database structure.

* We should add an authentication layer to our requests to ensure only verified merchants can submit payments and fetch them
  
* The models should be updated to include identifier for the merchant and shopper. This would enable us to ensure a merchant can only fetch payments related to them. Likewise, merchants would be able to use the shopper identifier to learn more about their transactions. For example, identifying loyal customers.
  
* We should introduce contract testing and request versioning at our service boundaries. This ensures any updates to our request models will be backwards compatible and any breaking changes will be introduced as part of a new api major version.

* We should introduce metrics and distributred tracing to the service. This will allow us to have better visibility on the health of the service and make it easier to trace the root cause of any bugs we might encounter.

* Given the sensitive nature of the data being transmitted, we introduce encryption for data in transfer
