USE [Overflow]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[insertNewAccountProc]
		@uName_proc_param = N'devin.cook@icloud.com',
		@pw_proc_param = N'1234567890',
		@fName_proc_param = N'Devin',
		@lName_proc_param = N'Cook'

SELECT	@return_value as 'Return Value'

GO
