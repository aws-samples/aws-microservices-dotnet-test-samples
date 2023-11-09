Feature: Order processing

    Scenario: process incoming order with enough quantity
        Given the following products are in the inventory
          | Id        | Name      | Quantity | Price |
          | product-1 | product 1 | 10       | 100   |
          | product-2 | product 2 | 20       | 200   |
        When a new message with arrives with the following products:
          | Id        |
          | product-1 |
          | product-2 |
        Then that order is saved as "ReadyForShipping" with items
          | ProductId | ItemStatus |
          | product-1 | Ready      |
          | product-2 | Ready      |

    Scenario: process incoming order with one item without quantity enough quantity
        Given the following products are in the inventory
          | Id        | Name      | Quantity | Price |
          | product-1 | product 1 | 10       | 100   |
          | product-2 | product 2 | 0        | 200   |
        When a new message with arrives with the following products:
          | Id        |
          | product-1 |
          | product-2 |
        Then that order is saved as "MissingItems" with items
          | ProductId | ItemStatus     |
          | product-1 | Ready          |
          | product-2 | NotInInventory |

    Scenario: process incoming order without items
        Given the following products are in the inventory
          | Id | Name | Quantity | Price |
        When a new message with arrives with the following products:
          | Id |
        Then that order is saved as "NoItemsInOrder" with items
          | ProductId | ItemStatus |