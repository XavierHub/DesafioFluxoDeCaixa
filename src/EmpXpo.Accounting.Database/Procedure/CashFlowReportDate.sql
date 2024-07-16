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

     SET NOCOUNT OFF;
END





