namespace PFM.Backend.Database.Entities
{
public enum Kind
    {
        dep, // Deposit
        wdw, // Withdrawal
        pmt, // Payment
        fee, // Fee
        inc, // Interest Credit
        rev, // Reversal
        adj, // Adjustment
        lnd, // Loan disburesement
        lnr, // Loan repayment
        fcx, // Foreign currency exchange
        aop, // Account openning
        acl, // Account closing
        spl, // Split payment
        sal  // Salary
    }
}