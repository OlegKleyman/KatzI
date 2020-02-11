@database
@web
Feature: AddBook
	In order to maintain my collection
	As a collector
	I want to be able to add books

Scenario: Add two numbers
	Given I am on the add page
	And I want to add a book
	| Title                                    | Author           | Description              | Rating | Series  |
	| Harry Potter and the Philosopher's Stone | JK Rowling       | Boy wizard               | 3      | Fantasy |
	When I press the submit button
	Then the book will be added to the database
