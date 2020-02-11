@database
@web
Feature: ViewBook
	In order to view book details
	As a collector
	I want a web page to view the book

Scenario: View Book details
	Given There exists a book
	When I view it
	Then I will see its details

Scenario: Similar books
	Given There are books
	| Title                                    | Author           | Description              | Rating | Series  |
	| Harry Potter and the Philosopher's Stone | JK Rowling       | Boy wizard               | 3      | Fantasy |
	| The Casual Vacancy                       | JK Rowling       | Another book             | 2      |         |
	| Lord Of The Rings                        | J. R. R. Tolkien | A mystical quest         | 4      | Fantasy |
	| How To Pass As Human                     | Nic Kelman       | Robot confused by humans | 5      |         |
	When I view "Harry Potter and the Philosopher's Stone"
	Then I will see the related books
	| Title              |
	| The Casual Vacancy |
	| Lord Of The Rings  |