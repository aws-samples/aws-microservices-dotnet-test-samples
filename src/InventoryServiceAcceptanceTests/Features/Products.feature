Feature: Product management

    Scenario: Add product to the inventory
        When a user add a new product "product 1" with price 100 and quantity 10
        Then that product is found in repository with name "product 1" price 100 and quantity 10

    Scenario: Add multiple products to the inventory
        Given a product "product 1" with price 100 and quantity 10 is in the inventory
        And a product "product 2" with price 200 and quantity 20 is in the inventory
        Then the following products are in the inventory
          | Name      | Quantity | Price |
          | product 1 | 10       | 100   |
          | product 2 | 20       | 200   |

    Scenario: Update product quantity
        Given a product "product 1" with price 100 and quantity 10 is in the inventory
        When user update product quantity to 20
        Then that product is found in repository with name "product 1" price 100 and quantity 20