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
         FROM [dbo].[CashFlow]  cf
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

    SET NOCOUNT OFF;
END





