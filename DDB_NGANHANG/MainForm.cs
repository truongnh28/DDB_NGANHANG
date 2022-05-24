﻿using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDB_NGANHANG
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        bool fl = false;
        int idx = 0;
        String nhom = "", chinhanh = "", manv = "";
        public static Stack undoNhanVien = new Stack();
        public static Stack undoKhachHang = new Stack();
        public MainForm()
        {
            InitializeComponent();
        }

        public static void EnableTab(TabPage page, bool enable)
        {
            EnableControls(page.Controls, enable);
        }
        private static void EnableControls(Control.ControlCollection ctls, bool enable)
        {
            foreach (Control ctl in ctls)
            {
                ctl.Enabled = enable;
                EnableControls(ctl.Controls, enable);
            }
        }

        private void xacNhanDangNhapBtn_Click(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)chiNhanhDangNhapComboBox.SelectedItem;
            DAO.servername = drv["TENSERVER"].ToString();
            DAO.username = taiKhoanDangNhapTxt.Text;
            DAO.password = matKhauDangNhapTxt.Text;
            if (DAO.KetNoi() == 1) return;
            String cmd = $"EXEC dbo.sp_DangNhap '{DAO.username}'";
            DataTable dt = DAO.ExecSqlDataTable(cmd, false);
            DAO.chiNhanhDT = dt;
            nhom = (string)dt.Rows[0][2];
            manv = (string)dt.Rows[0][0];
            maNVToolTrip.Text = "MaNV: " + (string)dt.Rows[0][0];
            hoTenToolTrip.Text = "Họ tên: " + (string)dt.Rows[0][1];
            nhomToolTrip.Text = "Nhóm: " + (string)dt.Rows[0][2];
            maNVToolTrip2.Text = "MaNV: " + (string)dt.Rows[0][0];
            hoTenToolTrip2.Text = "Họ tên: " + (string)dt.Rows[0][1];
            nhomToolTrip2.Text = "Nhóm: " + (string)dt.Rows[0][2];
            maNVToolTrip3.Text = "MaNV: " + (string)dt.Rows[0][0];
            hoTenToolTrip3.Text = "Họ tên: " + (string)dt.Rows[0][1];
            nhomToolTrip3.Text = "Nhóm: " + (string)dt.Rows[0][2];
            chinhanh = ((DataTable)chiNhanhDangNhapComboBox.DataSource).Rows[chiNhanhDangNhapComboBox.SelectedIndex][1].ToString().Equals("NHTRUONG\\SERVER2") ? "TANDINH" : "BENTHANH";
            //MessageBox.Show(chinhanh);
            //MessageBox.Show("Đăng nhập thành công");
            enableTagLogin(true);
            fl = true;
            if(dt.Rows[0][2].ToString().Equals("NganHang"))
            {
                khachHangPanel.Enabled = false;
                nhanVienPanel.Enabled = false;
            }
            if (!fl) return;
            if (nhom == "ChiNhanh")
            {
                chonChiNhanhLietKeKhachHangComboBox.Enabled = false;
                chonChiNhanhLietKeTaiKhoanComboBox.Enabled = false;
            }
            else
            {
                chonChiNhanhLietKeKhachHangComboBox.Enabled = true;
                chonChiNhanhLietKeTaiKhoanComboBox.Enabled = true;
                chonChiNhanhLietKeKhachHangComboBox.Items.Add("Chi nhánh Bến Thành");
                chonChiNhanhLietKeKhachHangComboBox.Items.Add("Chi nhánh Tân Định");
                chonChiNhanhLietKeTaiKhoanComboBox.Items.Add("Chi nhánh Bến Thành");
                chonChiNhanhLietKeTaiKhoanComboBox.Items.Add("Chi nhánh Tân Định");
                chonChiNhanhLietKeTaiKhoanComboBox.Items.Add("Toàn bộ chi nhánh");
            }
            process();
        }

        private void thoatDangNhapBtn_Click(object sender, EventArgs e)
        {
            taiKhoanDangNhapTxt.Text = "";
            matKhauDangNhapTxt.Text = "";
        }

        private void traCuuTCBtn_Click(object sender, EventArgs e)
        {
            TraCuuKhachHangForm fmm = new TraCuuKhachHangForm();
            fmm.StartPosition = FormStartPosition.CenterParent;
            fmm.ShowDialog(this);
        }
        private void process()
        {
            if (!fl) return;
            String cmd;
            DataTable dt;
            cmd = "EXEC [DBO].[SP_DANHSACHNHANVIEN]";
            dt = DAO.ExecSqlDataTable(cmd, false);
            nhanVienTable.DataSource = dt;
            cmd = "EXEC [DBO].[SP_DANHSACHKHACHHANG]";
            dt = DAO.ExecSqlDataTable(cmd, false);
            khachHangTable.DataSource = dt;
            //showInfoKhachHang();
            //showInfoNhanVien();
        }
        private void systemControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(systemControl.SelectedIndex == 0)
            {

            }
            else if(systemControl.SelectedIndex == 1)
            {
                
                
            }
            else if(systemControl.SelectedIndex == 2)
            {

            }
            process();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (DAO.ketNoiDBGoc() == 1) return;
            String cmd = "SELECT * FROM VIEW_DanhSachPhanManh";
            DAO.chiNhanhDT = DAO.ExecSqlDataTable(cmd, true);
            DAO.chiNhanhDT.Rows[1].Delete();
            DAO.chiNhanhDT.AcceptChanges();
            chiNhanhDangNhapComboBox.DataSource = DAO.chiNhanhDT;
            chiNhanhDangNhapComboBox.ValueMember = "TENSERVER";
            chiNhanhDangNhapComboBox.DisplayMember = "TENCN";
            DAO.conn.Close();
            enableTagLogin(false);
        }
        private void enableTagLogin(bool flag)
        {
            EnableTab(systemControl.TabPages[systemControl.SelectedIndex = 1], flag);
            EnableTab(systemControl.TabPages[systemControl.SelectedIndex = 2], flag);
            EnableTab(subSystemControl.TabPages[subSystemControl.SelectedIndex = 1], flag);
            EnableTab(subSystemControl.TabPages[subSystemControl.SelectedIndex = 2], flag);
            //EnableTab(subSystemControl.TabPages[subSystemControl.SelectedIndex = 3], flag);
            EnableTab(subSystemControl.TabPages[subSystemControl.SelectedIndex = 0], !flag);
            systemControl.SelectedIndex = 0;
            subSystemControl.SelectedIndex = 0;
            
        }
        private void taiKhoanTaoTaiKhoanTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void xacNhanDangXuatTxt_Click(object sender, EventArgs e)
        {
            enableTagLogin(false);
            taiKhoanDangNhapTxt.Text = "";
            matKhauDangNhapTxt.Text = "";
            maNVToolTrip.Text = "MaNV";
            hoTenToolTrip.Text = "Họ tên";
            nhomToolTrip.Text = "Nhóm";
        }

        private void xacNhanThoatBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void subQuanLyControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //idx = 0;
        }

        private void showInfoNhanVien(bool f = false)
        {
            int idx = 0;
            if (f) idx = nhanVienTable.CurrentRow.Index;
            if(nhanVienTable.Rows.Count > 0)
            {
                maNhanVienTxt.Text = nhanVienTable.Rows[idx].Cells[0].Value.ToString();
                hoNhanVienTxt.Text = nhanVienTable.Rows[idx].Cells[1].Value.ToString();
                tenNhanVienTxt.Text = nhanVienTable.Rows[idx].Cells[2].Value.ToString();
                diaChiNhanVienTxt.Text = nhanVienTable.Rows[idx].Cells[3].Value.ToString();
                sdtNhanVienTxt.Text = nhanVienTable.Rows[idx].Cells[5].Value.ToString();
                if (nhanVienTable.Rows[idx].Cells[4].Value.ToString().Equals("Nam"))
                {
                    namNhanVienRadio.Checked = true;
                    nuNhanVienRadio.Checked = !(namNhanVienRadio.Checked);
                }
                else
                {
                    nuNhanVienRadio.Checked = true;
                    namNhanVienRadio.Checked = !nuNhanVienRadio.Checked;
                }
            }
        }
        private void showInfoKhachHang(bool f = false)
        {
            if(khachHangTable.Rows.Count > 0)
            {
                cmndKhachHangTxt.Text = khachHangTable.Rows[idx].Cells[2].Value.ToString();
                hoKhachHangTxt.Text = khachHangTable.Rows[idx].Cells[0].Value.ToString();
                tenKhachHangTxt.Text = khachHangTable.Rows[idx].Cells[1].Value.ToString();
                diaChiKhachHangTxt.Text = khachHangTable.Rows[idx].Cells[3].Value.ToString();
                sdtKhachHangTxt.Text = khachHangTable.Rows[idx].Cells[5].Value.ToString();
                if (khachHangTable.Rows[idx].Cells[4].Value.ToString().Equals("Nam"))
                {
                    namKhachHangRadio.Checked = true;
                    nuKhachHangRadio.Checked = !(namKhachHangRadio.Checked);
                }
                else
                {
                    nuKhachHangRadio.Checked = true;
                    namKhachHangRadio.Checked = !nuKhachHangRadio.Checked;
                }
            }
        }
        private void khachHangTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idx = khachHangTable.CurrentRow.Index;
            showInfoKhachHang(true);
        }

        private void nhanVienTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idx = nhanVienTable.CurrentRow.Index;
            showInfoNhanVien(true);
        }

        private void themNhanVienBtn_Click(object sender, EventArgs e)
        {
            NhanVienForm nhanVienForm = new NhanVienForm(nhanVienTable.Rows[idx], true, chinhanh);
            nhanVienForm.StartPosition = FormStartPosition.CenterParent;
            nhanVienForm.ShowDialog(this);
            process();
        }

        private void xoaNhanVienBtn_Click(object sender, EventArgs e)
        {
            if (nhanVienTable.Rows[idx].Cells[0].Value.ToString().Equals(manv))
            {
                MessageBox.Show("Bạn không thể xóa chính mình");
                return;
            }
            if(MessageBox.Show("Bạn có thật sự muốn xóa nhân viên", "Xác nhận", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (nhanVienTable.Rows[idx].Cells[7].Value.ToString().Equals("1"))
                {
                    MessageBox.Show("Nhân viên này đã xóa hoặc ở chi nhánh khác");
                }
                else
                {
                    String MANV = nhanVienTable.Rows[idx].Cells[0].Value.ToString();
                    String cmd = $"UPDATE DBO.NhanVien SET TrangThaiXoa = 1 WHERE MANV = {MANV}";
                    //MessageBox.Show(cmd);
                    if (DAO.ExecSqlNonQuery(cmd, DAO.connstr) == 0)
                    {
                        undoNhanVien.Push($"UPDATE DBO.NhanVien SET TrangThaiXoa = 0 WHERE MANV = {MANV}");
                        MessageBox.Show("Thành công");
                    }
                }
            }
            process();
        }

        private void chinhSuaNhanVienBtn_Click(object sender, EventArgs e)
        { 
            NhanVienForm nhanVienForm = new NhanVienForm(nhanVienTable.Rows[idx], false, chinhanh);
            nhanVienForm.StartPosition = FormStartPosition.CenterParent;
            nhanVienForm.ShowDialog(this);
            process();
        }

        private void chuyenChiNhanhBtn_Click(object sender, EventArgs e)
        {
            if (nhanVienTable.Rows[idx].Cells[0].Value.ToString().Equals(manv))
            {
                MessageBox.Show("Bạn không thể chuyển chính mình");
                return;
            }
            if(nhanVienTable.Rows[idx].Cells[7].Value.ToString() == "1")
            {
                MessageBox.Show("Nhân viên này đã bị xóa hoặc đã nằm ở chi nhánh khác");
                return;
            }
            ChuyenChiNhanhForm chuyenChiNhanhForm = new ChuyenChiNhanhForm(nhanVienTable.Rows[idx].Cells[0].Value.ToString(), chinhanh);
            chuyenChiNhanhForm.StartPosition = FormStartPosition.CenterParent;
            chuyenChiNhanhForm.ShowDialog(this);
            process();
        }

        private void luuNhanVienBtn_Click(object sender, EventArgs e)
        {

        }

        private void traCuuTaoTaoTaiKhoanBtn_Click(object sender, EventArgs e)
        {
            DSNVForm dSNVForm = new DSNVForm();
            dSNVForm.StartPosition = FormStartPosition.CenterParent;
            dSNVForm.ShowDialog(this);
            if (DAO.nvChuaTaiKhoan != null)
            {
                nhanVienTaoTKTxt.Text = DAO.nvChuaTaiKhoan.Cells[0].Value.ToString();
                tenNVTaoTaiKhoanTxt.Text = DAO.nvChuaTaiKhoan.Cells[1].Value.ToString();
            }
        }

        private void subSystemControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(subSystemControl.SelectedIndex != 2)
            {
                nhanVienTaoTKTxt.Text = "";
                tenNVTaoTaiKhoanTxt.Text = "";
                taiKhoanTaoTaiKhoanTxt.Text = "";
                matKhauTaoTaiKhoanTxt.Text = "";
                xacNhanMatKhauTaoTaiKhoanTxt.Text = "";
            }
        }

        private void themKhachHangBtn_Click(object sender, EventArgs e)
        {
            KhachHangForm khachHangForm = new KhachHangForm(khachHangTable.Rows[idx], true, chinhanh);
            khachHangForm.StartPosition = FormStartPosition.CenterParent;
            khachHangForm.ShowDialog(this);
            process();
        }

        private void xoaKhachHangBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn xóa khách hàng", "Xác nhận", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (DAO.ExecSqlKiemTra1("SP_KIEMTRATAIKHOANKHACHHANG", khachHangTable.Rows[idx].Cells[2].Value.ToString()) == 1)
                {
                    MessageBox.Show("Không thể xóa khách hàng này");
                }
                else
                {
                    bool fc = false;
                    String cmd = $"DELETE FROM DBO.KhachHang WHERE CMND = {khachHangTable.Rows[idx].Cells[2].Value.ToString()}";
                    undoKhachHang.Push($"INSERT INTO DBO.KhachHang (CMND, HO, TEN, DIACHI, PHAI, NGAYCAP, SODT, MACN) VALUES (N'{khachHangTable.Rows[idx].Cells[2].Value.ToString()}', " +
                        $"N'{khachHangTable.Rows[idx].Cells[0].Value.ToString()}', N'{khachHangTable.Rows[idx].Cells[1].Value.ToString()}', " +
                        $"N'{khachHangTable.Rows[idx].Cells[3].Value.ToString()}', N'{khachHangTable.Rows[idx].Cells[4].Value.ToString()}', " +
                        $"N'{khachHangTable.Rows[idx].Cells[7].Value.ToString()}', N'{khachHangTable.Rows[idx].Cells[5].Value.ToString()}', N'{chinhanh}')");
                    if (DAO.ExecSqlNonQuery(cmd, DAO.connstr) == 0)
                    {
                        fc = true;
                        MessageBox.Show("Thành công");  
                    }
                    if(fc == false)
                    {
                        undoKhachHang.Pop();
                    }
                }
            }
            process();
        }

        private void chinhSuaKhachHangBtn_Click(object sender, EventArgs e)
        {
            KhachHangForm khachHangForm = new KhachHangForm(khachHangTable.Rows[idx], false, chinhanh);
            khachHangForm.StartPosition = FormStartPosition.CenterParent;
            khachHangForm.ShowDialog(this);
            process();
        }

        private void moTaiKhoangKhachHangBtn_Click(object sender, EventArgs e)
        {
            TaoSTKForm taoSTKForm = new TaoSTKForm(chinhanh);
            taoSTKForm.StartPosition = FormStartPosition.CenterParent;
            taoSTKForm.ShowDialog(this);
        }

        private void giaoDichKhachHangBtn_Click(object sender, EventArgs e)
        {
            GiaoDichForm giaoDichForm = new GiaoDichForm(manv, chinhanh);
            giaoDichForm.StartPosition = FormStartPosition.CenterParent;
            giaoDichForm.ShowDialog(this);
        }

        private void hoanTacNhanVienBtn_Click(object sender, EventArgs e)
        {
            if (undoNhanVien.Count == 0)
            {
                MessageBox.Show("Không còn hành động để hoàn tác");
                return;
            }
            String cmd = (String)undoNhanVien.Pop();
            if (DAO.ExecSqlNonQuery(cmd, DAO.connstr) != 0)
            {
                MessageBox.Show("Lỗi hoàn tác");
            }
            process();
        }

        private void hoanTacKhachHangBtn_Click(object sender, EventArgs e)
        {
            if (undoKhachHang.Count == 0)
            {
                MessageBox.Show("Không còn hành động để hoàn tác");
                return;
            }
            String cmd = (String)undoKhachHang.Pop();
            if (DAO.ExecSqlNonQuery(cmd, DAO.connstr) != 0)
            {
                MessageBox.Show("Lỗi hoàn tác");
            }
            process();
        }

        private void xacNhanSKBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(taiKhoanTCTxt.Text))
            {
                taiKhoanTCTxt.Focus();
                MessageBox.Show("Hãy điền vào ô số tài khoản, dùng nút 'Tra cứu' để tra cứu số tài khoản");
            }
            string ngayBD = thoiGianBD.Text;
            string ngayKT = thoiGianKT.Text;
            DateTime date1 = DateTime.Parse(ngayBD);
            DateTime date2 = DateTime.Parse(ngayKT);
            if (date1.Date <= date2.Date)
            {
                string taiKhoan = taiKhoanTCTxt.Text;
                string chiNhanhViet = (chinhanh == "TANDINH" ? "Tân Định" : "Bến Thành");
                ReportSaoKeGiaoDichTrongKhoangThoiGian report = new ReportSaoKeGiaoDichTrongKhoangThoiGian(ngayBD, ngayKT, taiKhoan, chiNhanhViet);
                ReportPrintTool print = new ReportPrintTool(report);
                print.ShowPreviewDialog();
                return;
            }
            thoiGianBD.Focus();
            MessageBox.Show("Thời gian bắt đầu không thể sau thời gian kết thúc");
            return;
        }

        private void xacNhanLietKeKhachHangBtn_Click(object sender, EventArgs e)
        {
            string chiNhanhViet = "";
            ReportLietKetKhachHangTheoTungChiNhanh report;
            ReportPrintTool print;
            if (nhom == "ChiNhanh")
            {
                chiNhanhViet = (chinhanh == "TANDINH" ? "TÂN ĐỊNH" : "BẾN THÀNH");
                report = new ReportLietKetKhachHangTheoTungChiNhanh(1, chiNhanhViet);
                print = new ReportPrintTool(report);
                print.ShowPreviewDialog();
                return;
            }
            string localChiNhanh = (chonChiNhanhLietKeKhachHangComboBox.SelectedIndex == 0 ? "BENTHANH" : "TANDINH");
            if (chinhanh != localChiNhanh)
            {
                chiNhanhViet = (localChiNhanh == "TANDINH" ? "TÂN ĐỊNH" : "BẾN THÀNH");
                report = new ReportLietKetKhachHangTheoTungChiNhanh(2, chiNhanhViet);
                print = new ReportPrintTool(report);
                print.ShowPreviewDialog();
                return;
            }
            chiNhanhViet = (localChiNhanh == "TANDINH" ? "TÂN ĐỊNH" : "BẾN THÀNH");
            report = new ReportLietKetKhachHangTheoTungChiNhanh(1, chiNhanhViet);
            print = new ReportPrintTool(report);
            print.ShowPreviewDialog();
            return;
        }

        private void xacNhanLietKeTaiKhoanBtn_Click(object sender, EventArgs e)
        {
            string chiNhanhViet;
            ReportLietKeTaiKhoanTrongKhoangThoiGianChiNhanh report;
            ReportPrintTool print;
            if (nhom == "ChiNhanh")
            {
                chiNhanhViet = (chinhanh == "TANDINH" ? "TÂN ĐỊNH" : "BẾN THÀNH");
                report = new ReportLietKeTaiKhoanTrongKhoangThoiGianChiNhanh(chonTGBDLietKe.Text, chonTGKTLietKe.Text, chinhanh, chiNhanhViet);
                print = new ReportPrintTool(report);
                print.ShowPreviewDialog();
                return;
            }
            int index = chonChiNhanhLietKeTaiKhoanComboBox.SelectedIndex;
            string localChiNhanh = (index == 0 ? "BENTHANH" : (index == 1 ? "TANDINH" : "HAICN"));
            chiNhanhViet = (localChiNhanh == "BENTHANH" ? "BẾN THÀNH" : (localChiNhanh == "TANDINH" ? "TÂN ĐỊNH" : "HAI CHI NHÁNH"));
            report = new ReportLietKeTaiKhoanTrongKhoangThoiGianChiNhanh(chonTGBDLietKe.Text, chonTGKTLietKe.Text, localChiNhanh, chiNhanhViet);
            print = new ReportPrintTool(report);
            print.ShowPreviewDialog();
            return;
        }

        private void xacNhanTaoTaiKhoanBtn_Click(object sender, EventArgs e)
        {
            if(Regex.IsMatch(taiKhoanTaoTaiKhoanTxt.Text, @"^[A-Za-z0-9]+$") == false)
            {
                MessageBox.Show("Tên đăng nhập chỉ nhận chữ cái và số");
                taiKhoanTaoTaiKhoanTxt.Text = "";
                taiKhoanTaoTaiKhoanTxt.Focus();
                return;
            }
            if (Regex.IsMatch(matKhauTaoTaiKhoanTxt.Text, @"^[A-Za-z0-9]+$") == false)
            {
                MessageBox.Show("Mật khẩu chỉ nhận chữ cái và số");
                matKhauTaoTaiKhoanTxt.Text = "";
                matKhauTaoTaiKhoanTxt.Focus();
                return;
            }
            if(xacNhanMatKhauTaoTaiKhoanTxt.Text.ToString().Equals(matKhauTaoTaiKhoanTxt.Text.ToString()) == false)
            {
                MessageBox.Show("Xác nhận mật khẩu không thành công");
                xacNhanMatKhauTaoTaiKhoanTxt.Text = "";
                xacNhanMatKhauTaoTaiKhoanTxt.Focus();
                return;
            }
            String cmd = $"EXEC SP_TAOTAIKHOAN {nhanVienTaoTKTxt.Text.Trim()}, {xacNhanMatKhauTaoTaiKhoanTxt.Text.Trim()}, {taiKhoanTaoTaiKhoanTxt.Text.Trim()}, {nhom.Trim()}";
            //MessageBox.Show(cmd);
            if(DAO.ExecSqlNonQuery(cmd, DAO.connstr) == 0)
            {
                MessageBox.Show("Tạo tài khoản thành công");
            }
            nhanVienTaoTKTxt.Text = "";
            tenNVTaoTaiKhoanTxt.Text = "";
            taiKhoanTaoTaiKhoanTxt.Text = "";
            matKhauTaoTaiKhoanTxt.Text = "";
            xacNhanMatKhauTaoTaiKhoanTxt.Text = "";
        }

        private void thoatTaoTaiKhoanBtn_Click(object sender, EventArgs e)
        {
            nhanVienTaoTKTxt.Text = "";
            tenNVTaoTaiKhoanTxt.Text = "";
            taiKhoanTaoTaiKhoanTxt.Text = "";
            matKhauTaoTaiKhoanTxt.Text = "";
            xacNhanMatKhauTaoTaiKhoanTxt.Text = "";
        }
    }
}