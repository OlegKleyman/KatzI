@database
@web
Feature: UpdateBook
	In order to maintain my collection
	As a collector
	I want to be able to update my books

Scenario: Update book
	Given There exists a book
	And I am on the update page
	When I update the book
	| Title                                    | Author           | Description              | Rating | Series  |
	| Harry Potter and the Philosopher's Stone | JK Rowling       | Boy wizard               | 3      | Fantasy |
	Then the book will be updated in the database
