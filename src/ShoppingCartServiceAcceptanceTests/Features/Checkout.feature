Feature: Checkout and create order
Simple calculator for adding two numbers

    Scenario: Checkout existing shopping cart
        Given a user creates a new shopping cart
        And the following items exist in inventory:
          | Id     | Name      | Quantity | Price |
          | prod-1 | product 1 | 10       | 100   |
        And a user adds item "prod-1" to shopping cart
        When a user creates an order using checkout
        Then a new message is queued with item "prod-1"