USE [master]
GO

CREATE DATABASE [DbCashFlow]
GO

USE [DbCashFlow]
GO

CREATE TABLE [dbo].[CashFlow]
(
   [Id]                    INT           PRIMARY KEY  IDENTITY   NOT NULL,	
   [Type]                  INT                                   NOT NULL,
   [Amount]                DECIMAL(18, 2)                        NOT NULL,
   [Description]           VARCHAR(100)                          NOT NULL,
   [CreatedOn]             SMALLDATETIME                         NOT NULL,    
   CHECK ([Type]=(0) OR [Type]=(1)), -- 0:'Debit', 1:'Credit'
)
GO
CREATE INDEX [Ind_CashFlow_1] ON [dbo].[CashFlow] ([CreatedOn])
GO


CREATE PROCEDURE [dbo].[CashFlowReport](   
   @StartDate     SMALLDATETIME,
   @EndDate       SMALLDATETIME
)
AS
BEGIN 
      SET NOCOUNT ON;

      /* Ex.:
            Procedure que retorna o valor de debito e credito de acordo com ranger das datas especificadas

            EXEC [dbo].[CashFlowReport] @StartDate='20240713', @EndDate = '20240715';
            EXEC [dbo].[CashFlowReport] @StartDate='20240701', @EndDate = '20240801';
      */      
      
      WITH CashFlow_cte([Type], [Amount])
      AS
      (
         SELECT 
               CASE WHEN cf.[Type] = 0 THEN 'Debit' ELSE 'Credit' END 'Type',
               cf.[Amount]
         FROM [dbo].[CashFlow] cf
         WITH (NOLOCK)
        WHERE cf.[CreatedOn] BETWEEN @StartDate AND @EndDate
      )
      SELECT 
              [Debit], 
              [Credit],
              [Debit] + [Credit] AS Total
        FROM CashFlow_cte
       PIVOT (
               SUM([Amount])
               FOR [Type] IN([Debit], [Credit])
             ) AS pvt

    SET NOCOUNT OFF
END

GO

CREATE PROCEDURE [dbo].[CashFlowReportDate]
AS
BEGIN
      SET NOCOUNT ON;
      /* Ex.:
            Procedure que retorna as datas disponíveis para geração do relatório

            EXEC [dbo].[CashFlowReportDate]            
      */      
     
       SELECT DISTINCT
              FORMAT(cf.CreatedOn, 'yyyy-MM-dd 00:00:00') CreatedOn
         FROM [dbo].[CashFlow] cf
         WITH (NOLOCK)
     GROUP BY CreatedOn

     SET NOCOUNT OFF
END