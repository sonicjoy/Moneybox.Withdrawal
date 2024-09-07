# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WithdrawMoney.Execute(...).

## Guidelines

* The test should take about an hour to complete, although there is no strict time limit
* You should fork or copy this repository into your own public repository (Github, BitBucket etc.) before you do your work
* Your solution must build and any tests must pass
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed test, zip up your solution, excluding any build artifacts to reduce the size, and email it back to our recruitment team.

Good luck!

---

# Solution

The solution contains an additional folder `tests` which contains the unit tests for the core logic, apart from the original `src` folder.
The solution requires .NET Core 8.0 to run the tests.

## How to build and test the solution

### To build and test the solution, follow these steps:

1. Open the solution in Visual Studio or your preferred IDE.
2. Build the solution to ensure that all dependencies are resolved and the project compiles successfully.
3. Open the test project located in the `tests` folder.
4. Run the unit tests using a test runner or the built-in test explorer in your IDE.
5. Verify that all tests pass without any failures or errors.

### To build and test the solution using the command line interface (CLI), follow these steps:

1. Open a terminal or command prompt.
2. Navigate to the root folder of the solution (`/c:/Users/sonic/source/repos/MoneyBox/moneybox-withdrawal-master`).
3. Build the solution by running the following command:
    ```
    dotnet build
    ```
    This will compile the project and resolve any dependencies.
4. Navigate to the test project folder (`/c:/Users/sonic/source/repos/MoneyBox/moneybox-withdrawal-master/tests`).
5. Run the unit tests using the following command:
    ```
    dotnet test
    ```
    This will execute the tests and provide the test results.
6. Verify that all tests pass without any failures or errors.

Make sure you have the .NET Core SDK installed on your machine before running the above commands.


