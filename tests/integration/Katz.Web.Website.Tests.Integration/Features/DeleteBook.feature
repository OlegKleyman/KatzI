@database
@web
Feature: DeleteBook
	In order to keep track of my book collection
	As a collector
	I want to delete books when I no longer have them

Scenario: Delete book
	Given There exists a book
	When I view it
	And click the delete link
	Then it will be deleted
