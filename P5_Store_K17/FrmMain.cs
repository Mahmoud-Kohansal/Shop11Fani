using System;
using System.Windows.Forms;

namespace P5_Store_K17
{
    public partial class FrmMain : Form
    {
        public int NewFactorId { get; set; }
        public int SelectedGoodsTotalPrice { get; set; }
        public decimal TotalPay { get; set; }
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.customersTableAdapter.Fill(this.storeDataSet.Customers);
            this.goodsTableAdapter.Fill(this.storeDataSet.Goods);

        }

        private void BtnInsertFactor_Click(object sender, EventArgs e)
        {
            NewFactor();
        }
        private void NewFactor()
        {
            int? factorsCount = factorsTableAdapter.Count();
            NewFactorId = factorsCount.HasValue ? factorsCount.Value + 1 : 0;
            LblFactoNo.Text = NewFactorId.ToString();
            LblFactorDate.Text = DateTime.Now.ToString("yyy-MM-dd");
            ResetTotalPay();
        }

        private void BtnNewFactorCustomerTab_Click(object sender, EventArgs e)
        {
            //TxtCustomerId.Text = LblCustomerIdCustomerPage.Text;
            //LblCustomerName.Text = LblCustomerNameCustomerTab.Text;
            TbCtrlShop.SelectedTab = TbPgFactor;
            NewFactor();
        }

        private void TbCtrlShop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(TbCtrlShop.SelectedTab == TbPgGood)
            {
                goodsTableAdapter.Fill(storeDataSet.Goods);
            }
        }

        private void BtnGoodsInsert_Click(object sender, EventArgs e)
        {
            if (TxtGoodsNameInsert.Text.Length > 0
                && int.TryParse(TxtGoodsUnitPriceInsert.Text, out int unitPrice)
                && int.TryParse(TxtGoodsStockInsert.Text, out int goodsStock)
                )
            {
                storeDataSet.GoodsRow newGoodsRow = storeDataSet.Goods.NewGoodsRow();
                newGoodsRow.GoodsName = TxtGoodsNameInsert.Text;
                newGoodsRow.GoodsUnitPrice = unitPrice;
                newGoodsRow.GoodStock = goodsStock;
                storeDataSet.Goods.Rows.Add(newGoodsRow);
                goodsTableAdapter.Update(storeDataSet.Goods);
                storeDataSet.AcceptChanges();
                goodsTableAdapter.Fill(storeDataSet.Goods);
            }            

        }

        private void BtnCustomer3Dot_Click(object sender, EventArgs e)
        {
            TbCtrlShop.SelectedTab = TbPgCustomer;
        }

        private void BtnInsertCustomer_Click(object sender, EventArgs e)
        {
            if (TxtCustomerNameInsert.Text.Length > 0
                && TxtCustomerLastNameInsert.Text.Length > 0
                && TxtCustomerMobileInsert.Text.Length > 0
                )
            {
                // Insert into customer Table
                customersTableAdapter.InsertQuery(TxtCustomerNameInsert.Text, TxtCustomerLastNameInsert.Text, TxtCustomerMobileInsert.Text);
                customersTableAdapter.Fill(storeDataSet.Customers);
            }
        }

        private void BtnAddToFactor_Click(object sender, EventArgs e)
        {
            if (int.TryParse(TxtGoodsNo.Text, out int goodsNumber) && SelectedGoodsTotalPrice > 0)
            {
                //Add to Factor
                int n = DgvFactor.RowCount-1;
                DgvFactor.Rows.Add();
                //GoodsCode
                DgvFactor.Rows[n].Cells["GoodsCode"].Value = TxtGoodsCode.Text;

                //GoodsName
                DgvFactor.Rows[n].Cells["GoodsName"].Value = LblGoodsName.Text;

                //No
                DgvFactor.Rows[n].Cells["No"].Value = TxtGoodsNo.Text;

                //UnitPrice
                DgvFactor.Rows[n].Cells["UnitPrice"].Value = LblGoodsUnitPrice.Text;

                //TotalOfOne
                DgvFactor.Rows[n].Cells["TotalOfOne"].Value = SelectedGoodsTotalPrice;


                UpdateTotalPay(SelectedGoodsTotalPrice, true);

            }
                
        }

        private void UpdateTotalPay(int selectedGoodsTotalPrice, bool increase)
        {
            TotalPay += increase ? selectedGoodsTotalPrice : (-1*selectedGoodsTotalPrice);
            LblTotalPay.Text = TotalPay.ToString();
        }

        private void BtnFactor3DotGoods_Click(object sender, EventArgs e)
        {
            TbCtrlShop.SelectedTab = TbPgGood;
        }

        private void TxtGoodsNo_TextChanged(object sender, EventArgs e)
        {
            SelectedGoodsTotalPrice = 0;
            if (int.TryParse(TxtGoodsNo.Text, out int goodsNumber))
                SelectedGoodsTotalPrice = goodsNumber * int.Parse(LblGoodsUnitPrice.Text);
        }

        private void DgvFactor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgvSender = (DataGridView)sender;
            if(dgvSender.Columns[e.ColumnIndex] is DataGridViewImageColumn && e.RowIndex >= 0)
            {
                if (!dgvSender.Rows[e.RowIndex].IsNewRow)
                {
                    UpdateTotalPay(int.Parse(dgvSender.Rows[e.RowIndex].Cells["TotalOfOne"].Value.ToString()), increase: false);
                    dgvSender.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void BtnAddGoodsToFactor_Click(object sender, EventArgs e)
        {
            
            TbCtrlShop.SelectedTab = TbPgFactor;
            TxtGoodsNo.Text = "1";
        }

        private void TxtCustomerId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckCustomerCode();
            }
        }
        private void CheckCustomerCode()
        {
            if (int.TryParse(TxtCustomerId.Text, out int customerId))
            {
                var customer = customersTableAdapter.GetCustomerById(customerId);
                if (customer.Count > 0)
                {
                    LblCustomerName.Text = customer[0].CustomerFirstName;
                    LblCustomerLastName.Text = customer[0].CustomerLastName;
                    LblCustomerMobile.Text = customer[0].CustomerMobile;
                }
                else
                {
                    LblCustomerName.Text = string.Empty;
                    LblCustomerLastName.Text = string.Empty;
                    LblCustomerMobile.Text = string.Empty;
                    MessageBox.Show("مشتری موجود نیست", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                LblCustomerName.Text = string.Empty;
                LblCustomerLastName.Text = string.Empty;
                LblCustomerMobile.Text = string.Empty;
                MessageBox.Show("برای کد مشتری یک عدد وارد کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void TxtCustomerId_Leave(object sender, EventArgs e)
        {
            CheckCustomerCode();
        }

        private void TxtGoodsCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckGoodsCode();
            }

        }

        private void TxtGoodsCode_Leave(object sender, EventArgs e)
        {
            CheckGoodsCode();
        }
        private void CheckGoodsCode()
        {
            if (int.TryParse(TxtGoodsCode.Text, out int goodsId))
            {
                var goods = goodsTableAdapter.GetGoodsById(goodsId);
                if (goods.Count > 0)
                {
                    LblGoodsName.Text = goods[0].GoodsName;
                    LblGoodsUnitPrice.Text = goods[0].GoodsUnitPrice.ToString();
                    LblGoodsStockInventory.Text = goods[0].GoodStock.ToString();
                    TxtGoodsNo.Text = "1";
                }
                else
                {

                    MessageBox.Show("کالا موجود نیست", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LblGoodsName.Text = string.Empty;
                    LblGoodsUnitPrice.Text = string.Empty;
                    LblGoodsStockInventory.Text = string.Empty;
                    TxtGoodsNo.Text = "0";
                }
            }
            else
            {
                MessageBox.Show("برای کد کالا یک عدد وارد کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LblGoodsName.Text = string.Empty;
                LblGoodsUnitPrice.Text = string.Empty;
                LblGoodsStockInventory.Text = string.Empty;
                TxtGoodsNo.Text = "0";
            }
        }

        private void BtnMakeNewFactor_Click(object sender, EventArgs e)
        {
            NewFactor();
        }

        private void BtnRestFactor_Click(object sender, EventArgs e)
        {
            DgvFactor.Rows.Clear();
            ResetTotalPay();
        }
        private void ResetTotalPay()
        {
            TotalPay = 0;
            LblTotalPay.Text = TotalPay.ToString();
        }

        private void TxtSearchGoods_TextChanged(object sender, EventArgs e)
        {
            if (TxtSearchGoods.Text.Length == 0)
            {
                goodsTableAdapter.Fill(storeDataSet.Goods);
            }
            else
            {
                goodsTableAdapter.FillByName(storeDataSet.Goods,TxtSearchGoods.Text);
            }
        }

        private void TxtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            if (TxtSearchCustomer.Text.Length == 0)
            {
                customersTableAdapter.Fill(storeDataSet.Customers);
            }
            else
            {
                customersTableAdapter.FillByName(storeDataSet.Customers, TxtSearchCustomer.Text, TxtSearchCustomer.Text);
            }
        }
    }
}
