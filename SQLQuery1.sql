DECLARE @0 NVARCHAR(MAX) = N'Символы, святыни и награды Российской державы. часть 312412432312341'
		,@1 NVARCHAR(MAX) = N'Кузнецов Александр'
		,@2 NVARCHAR(MAX) = N'Казакевич Александр'
		,@3 NVARCHAR(MAX) = N'Соболева Надежда'
		,@4 NVARCHAR(MAX) = N'Балязин Вольдемар Николаевич'

		SELECT b.Title,COUNT(a.AuthorId) AS Authors FROM Books b, Authorship asps, Authors a 
		WHERE a.AuthorId = asps.Id_Author AND b.BookId = asps.Id_Book AND b.Title = @0 AND a.FullName IN(@1,@2,@3,@4) 