@database
@web
Feature: SearchBooks
	In order to make sense of my collection
	As a collector
	I want to be able to search my collection

Scenario: Search for books without title
	Given There are books
	| Title          | Author     | Description       | Rating | Series  |
	| Harry Potter 1 | JK Rowling | Boy wizard        | 4      | harry   |
	| Harry Potter 2 | JK Rowling | Limited Edition 1 | 3      | Fantasy |
	| Harry Potter 3 | JK Rowling | Limited Edition 2 | 4      | Fantasy |
	| Harry Potter 4 | JK Rowling | Limited Edition 3 | 4      | harry   |
	| Harry Potter 5 | JK Rowling | Limited Edition 4 | 5      | Fantasy |
	And I am on the search page
	When I search for
	| Author     | Title | Rating | Series |
	| JK Rowling |       | 4      | harry  |
	Then I will see the following results
	| Title          | Author     | Rating | Series |
	| Harry Potter 1 | JK Rowling | 4      | harry  |
	| Harry Potter 4 | JK Rowling | 4      | harry  |

Scenario: Search for books with title
	Given There are books
	| Title        | Author        | Description   | Rating | Series |
	| Book         | JK Rowling    | description   | 4      | harry  |
	| Book         | Somebody Else | description 2 | 3      | Action |
	| Another Book | Somebody Else | description 3 | 3      |        |
	And I am on the search page
	When I search for
	| Title |
	| Book |
	Then I will see the following results
	| Title | Author        | Rating | Series |
	| Book  | JK Rowling    | 4      | harry  |
	| Book  | Somebody Else | 3      | Action |