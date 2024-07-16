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
