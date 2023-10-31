Feature: Shopping cart management

    Scenario: Create new shopping cart
        Given a user creates a new shopping cart
        When a user queries for that shopping cart
        Then the shopping cart is found in repository

    Scenario: Add item to shopping cart
        Given a user creates a new shopping cart
        And the following items exist in inventory:
          | Id     | Name      | Quantity | Price |
          | prod-1 | product 1 | 10       | 100   |
        When a user adds item "prod-1" to shopping cart
        Then the following items exists in that shopping cart: "prod-1"

    Scenario: Add item no existing item to shopping cart
        Given a user creates a new shopping cart
        And the product "prod-1" is not in the inventory
        When a user adds item "prod-1" to shopping cart
        Then then return not found