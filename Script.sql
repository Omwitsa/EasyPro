// Transporters
SELECT TransCode, TransName, CertNo, Locations, TregDate, email, Phoneno, Town, Address, Subsidy, Accno, Bcode, BBranch, Active, 'ELBURGON', auditid, GETDATE(), isfrate, rate, canno, tt, 'ELBURGON PROGRESSIVE DAIRY FCS', ttrate, BR, status1, 'Monthly', NULL FROM d_Transporters 

// Farmers
SELECT NULL, SNo, Regdate, IdNo, Names, AccNo, Bcode, BBranch, Type, Village, Location, Division, District, 'NAKURU', Trader, 1, 1,
'ELBURGON', PhoneNo, Address, Town,Email, 'Monthly', sign, photo, AuditId, GETDATE(), 'ELBURGON PROGRESSIVE DAIRY FCS',
Loan, Compare, isfrate, frate, rate, hast, Br, mno, branchcode, NULL, NULL, aarno, tmd, 0, thcpactive, thcppremium, status, status2, status3,
status4, status5, status6, 'MILK', dob, 0, 0, status1, 0, NULL
FROM d_Suppliers

-- UPDATE d_Suppliers SET Approval = 1

//Transporter Assignment
SELECT Trans_Code, sno, Rate, startdate, Active, DateInactivate, auditid, GETDATE(), isfrate, 'ELBURGON PROGRESSIVE DAIRY FCS', 'Milk', 'ELBURGON', NULL, NULL FROM d_Transport
 
// Intake
SELECT SNo, TransDate,TransTime, 'Milk', QSupplied, PPU, PAmount, 0, 0, 'Intake', Paid, remark, 1, 'Wilson', GETDATE(), 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS', 'EP-E-001', 'EP-L-001', 0, NULL, NULL FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-31 00:00:00.000' ORDER BY TransDate

// Agrovet products
SELECT p_code, p_name, S_No, Qin, Qout, Date_Entered, Last_D_Updated, user_id, GETDATE(), o_bal, SupplierID, Serialized, unserialized, seria, pprice, sprice, Branch, 'EP-A-18', 'EP-I-001', AI, NULL, 0, 1, 1, 'RE', 'ELBURGON PROGRESSIVE DAIRY FCS' FROM ag_Products

// Agrovet RECEIPTS
select R_No, P_code, T_Date, Amount, S_No, Qua, S_Bal, user_id, GETDATE(), Cash, SNO, Transby, Idno, mobile, Remarks, 'ELBURGON', 0, 0, 0, 0, 0, 0, '', 'ELBURGON PROGRESSIVE DAIRY FCS', NULL from ag_Receipts 

// Agrovet Product intake
SELECT SNO, T_Date, '00:00:00.0000000', 'AGROVET', '0', '0', '0', Amount, '0', 'AGROVET', '0', 'AGROVET', '2', 'admin', GETDATE(), 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS', 'EP-A-18', 'EP-I-001', NULL, NULL, NULL FROM ag_Receipts WHERE saccocode = 'ELBURGON PROGRESSIVE DAIRY FCS' AND Branch = 'ELBURGON' AND (T_Date BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-31 00:00:00.000')

// transporter balancing
SELECT CAST(auditdatetime AS date), Trans_Code, QNT, QNTY, 0, 0, 0, 'ELBURGON PROGRESSIVE DAIRY FCS', 'ELBURGON' FROM d_TransDetailed ORDER BY ID DESC

// Supplier Deductions
SELECT SNo, Date_Deduc, '00:00:00.0000000', Description, 0, 0, 0, Amount, 0, Description, 0, Remarks, 2, auditid, GETDATE(), 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS', 'EP-L-001', 'EP-A-005', 0, NULL, NULL FROM d_supplier_deduc WHERE EndDate = '2023-08-31 00:00:00.000' AND Description IN('advance', 'CLINICAL')

// Transporter Deductions
SELECT TransCode, TDate_Deduc, '00:00:00.0000000', Description, 0, 0, 0, Amount, 0, Description, 1, Remarks, 2, auditid, GETDATE(), 'ELBURGON', 'ELBURGON PROGRESSIVE DAIRY FCS', 'EP-L-001', 'EP-A-005', 0, NULL, NULL FROM d_Transport_Deduc WHERE startdate = '2023-08-01 00:00:00.000' AND enddate = '2023-08-31 00:00:00.000'

// Shares
d_sconribution, CONTRIB, d_SharesReport, d_Shares

// Bonus
d_Bonus

// Pricing
SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-31 00:00:00.000' ORDER BY TransDate

SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-01 00:00:00.000' AND '2023-07-15 00:00:00.000' AND PPU != '44.00'
SELECT * FROM d_Milkintake WHERE TransDate BETWEEN '2023-07-16 00:00:00.000' AND '2023-07-22 00:00:00.000' AND PPU != '44.00'
-- 2023-07-01 00:00:00.000 -> 2023-07-15 00:00:00.000 = 44
-- 2023-07-16 00:00:00.000 -> 2023-07-22 00:00:00.000 = 42
-- 2023-07-01 00:00:00.000 > 44
