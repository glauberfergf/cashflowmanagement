Create DATABASE  IF NOT EXISTS CashFlowManagement;

use CashFlowManagement;


CREATE TABLE  IF NOT EXISTS Payment (
    Id char(36) PRIMARY KEY,
    TransactionId char(36) NOT NULL,
    PaymentOrigin INT NOT NULL,
    Status INT NOT NULL,
    PaymentType INT NOT NULL,
    PaymentDescription VARCHAR(255) DEFAULT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    CreatedDate DATETIME NOT NULL
);