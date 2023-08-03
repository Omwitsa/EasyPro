// Transporters
SELECT TransCode, TransName, CertNo, Locations, TregDate, email, Phoneno, Town, Address, Subsidy, Accno, Bcode, BBranch, Active, TBranch, auditid, auditdatetime, isfrate, rate, canno, tt, 'ELBURGON PROGRESSIVE DAIRY FCS', ttrate, BR, status1, 'Monthly', NULL FROM d_Transporters 

// Farmers
SELECT NULL, SNo, Regdate, IdNo, Names, AccNo, Bcode, BBranch, Type, Village, 'ELBURGON', 'ELBURGON', 'MOLO', 'NAKURU', Trader, active, Approval,
'ELBURGON', PhoneNo, 'BOX 133 ELBURGON', 'ELBURGON',Email, 'Monthly', sign, photo, AuditId, auditdatetime, 'ELBURGON PROGRESSIVE DAIRY FCS',
Loan, Compare, isfrate, frate, rate, hast, Br, mno, branchcode, NULL, NULL, aarno, tmd, 0, thcpactive, thcppremium, status, status2, status3,
status4, status5, status6, 'MILK', dob, 0, 0, status1, 0, NULL
FROM d_Suppliers

//Transporter Assignment
SELECT Trans_Code, sno, Rate, startdate, Active, DateInactivate, auditid, auditdatetime, isfrate, 'ELBURGON PROGRESSIVE DAIRY FCS', 'Milk', 'ELBURGON', NULL, NULL FROM d_Transport
 
// Intake
SELECT SNo, TransDate,TransTime, 'Milk', QSupplied, PPU, PAmount, 0, 0, 'Intake', Paid, remark, 1, 'Wilson', auditdatetime, 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS', 'EP-E-001', 'EP-L-001', 0, NULL, NULL FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-31 00:00:00.000' ORDER BY TransDate

// Agrovet products
SELECT p_code, p_name, S_No, Qin, Qout, Date_Entered, Last_D_Updated, user_id, audit_date, o_bal, SupplierID, Serialized, unserialized, seria, pprice, sprice, Branch, 'EP-A-18', 'EP-I-001', AI, NULL, 0, 1, 1, 'RE', 'ELBURGON PROGRESSIVE DAIRY FCS' FROM ag_Products

// Agrovet RECEIPTS
select R_No, P_code, T_Date, Amount, S_No, Qua, S_Bal, user_id, audit_date, Cash, SNO, Transby, Idno, mobile, Remarks, Branch, 0, 0, 0, 0, 0, 0, '', 'ELBURGON PROGRESSIVE DAIRY FCS', NULL from ag_Receipts 


// deductions
SELECT SNo, deduction, Remark, StartDate, Rate, Stopped, Auditdatetime, AuditId, Rated, status, status2, status3, status4, status5, status6, 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS' FROM d_PreSets 
SELECT sno, bal, IdNo, Code, Name,Sex, Loc, Type,TransDate, pmode, Cash, Period, Amnt, AuditId, AuditDateTime, Shares, Regdate, Mno, amount, PREMIUM, spu, 'ELBURGON PROGRESSIVE DAIRY FCS', NULL FROM d_Shares

// Pricing
SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-31 00:00:00.000' ORDER BY TransDate

SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-15 00:00:00.000' AND PPU != '44.00'
SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-16 00:00:00.000' AND '2023-07-22 00:00:00.000' AND PPU != '44.00'
-- 2023-07-01 00:00:00.000 -> 2023-07-15 00:00:00.000 = 44
-- 2023-07-16 00:00:00.000 -> 2023-07-22 00:00:00.000 = 42
-- 2023-07-01 00:00:00.000 > 44
