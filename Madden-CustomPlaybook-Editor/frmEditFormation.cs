using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MaddenCustomPlaybookEditor
{
    public partial class frmEditFormation : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private frmPlaybook playbookFormRef;
        public List<SETL> DefaultSETL;
        public List<SETP> DefaultSETP;
        public List<SETG> DefaultSETG;

        public frmEditFormation(frmPlaybook playbookFormHandle)
        {
            playbookFormRef = playbookFormHandle;
            InitializeComponent();
            AddDrag(mnuPBPL);
        }

        private void frmEditFormation_Load(object sender, EventArgs e)
        {
            playbookFormRef.cbxFormations.Enabled = false;
            playbookFormRef.cbxSubFormations.Enabled = false;
            playbookFormRef.cbxAlignments.Enabled = false;
            playbookFormRef.btnEditFormation.Enabled = false;
            playbookFormRef.btnEditPlayData.Enabled = false;
            playbookFormRef.chbFlipPlay.Enabled = false;

            setSETL();
            setSETP();
            setSETG();
        }

        private void frmEditFormation_FormClosed(object sender, FormClosedEventArgs e)
        {
            playbookFormRef.cbxFormations.Enabled = true;
            playbookFormRef.cbxSubFormations.Enabled = true;
            playbookFormRef.btnEditFormation.Enabled = true;
            playbookFormRef.btnEditPlayData.Enabled = true;
            if (playbookFormRef.mnuOptions_Playart_PSAL.Checked)
            {
                playbookFormRef.chbFlipPlay.Enabled = true;
                playbookFormRef.cbxAlignments.Enabled = true;
            }
            else
            {
                playbookFormRef.chbFlipPlay.Enabled = false;
                playbookFormRef.cbxAlignments.Enabled = false;
            }
        }

        #region Title Bar

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Close_MouseEnter(object sender, EventArgs e)
        {
            btn_Close.Image = ((Image)(Properties.Resources.btn_close_hover));
        }

        private void btn_Close_MouseLeave(object sender, EventArgs e)
        {
            btn_Close.Image = ((Image)(Properties.Resources.btn_close));
        }

        private void AddDrag(Control Control)
        {
            Control.MouseDown += new MouseEventHandler(DragForm_MouseDown);
        }

        private void DragForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        private void btnResetSETL_Click(object sender, EventArgs e)
        {
            playbookFormRef.Play.SETL = (from row in DefaultSETL
                                         select new SETL()
                                         {
                                             rec = row.rec,
                                             setl = row.setl,
                                             FORM = row.FORM,
                                             MOTN = row.MOTN,
                                             CLAS = row.CLAS,
                                             SETT = row.SETT,
                                             SITT = row.SITT,
                                             SLF_ = row.SLF_,
                                             name = row.name,
                                             poso = row.poso
                                         }).Cast<SETL>().ToList();

            dgvSETL.AutoResizeColumns();
            int SETLrec = ((SETL)playbookFormRef.cbxSubFormations.SelectedItem).rec;
            int PBPLrec = ((PBPL)playbookFormRef.cbxPlays.SelectedItem).rec;

            playbookFormRef.SetPlay(playbookFormRef.Play);
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdateSubFormations();
            playbookFormRef.UpdateAlignments();
            playbookFormRef.UpdatePlays();

            playbookFormRef.cbxSubFormations.SelectedItem = (playbookFormRef.cbxSubFormations.Items.Cast<SETL>().ToList()).Find(item => item.rec == SETLrec);
            playbookFormRef.cbxPlays.SelectedItem = (playbookFormRef.cbxPlays.Items.Cast<PBPL>().ToList()).Find(item => item.rec == PBPLrec);

            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            dgvSETL.DataSource = playbookFormRef.Play.SETL;

            Program.ResizeDataGrid(dgvSETL);
            Focus();
        }

        private void btnResetSETP_Click(object sender, EventArgs e)
        {
            playbookFormRef.Play.SETP = (from row in DefaultSETP select new SETP()
            {
                rec = row.rec,
                SETL = row.SETL,
                setp = row.setp,
                SGT_ = row.SGT_,
                arti = row.arti,
                opnm = row.opnm,
                tabo = row.tabo,
                poso = row.poso,
                odep = row.odep,
                flas = row.flas,
                DPos = row.DPos,
                EPos = row.EPos,
                fmtx = row.fmtx,
                artx = row.artx,
                fmty = row.fmty,
                arty = row.arty
            }).Cast<SETP>().ToList();
            playbookFormRef.SetPlay(playbookFormRef.Play);
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            dgvSETP.DataSource = playbookFormRef.Play.SETP;

            Program.ResizeDataGrid(dgvSETP);
            Focus();
        }

        private void btnResetSETG_Click(object sender, EventArgs e)
        {
            playbookFormRef.SETG = (from row in DefaultSETG select new SETG()
            {
                rec = row.rec,
                SETP = row.SETP,
                SGF_ = row.SGF_,
                SF__ = row.SF__,
                x___ = row.x___,
                y___ = row.y___,
                fx__ = row.fx__,
                fy__ = row.fy__,
                anm_ = row.anm_,
                dir_ = row.dir_,
                fanm = row.fanm,
                fdir = row.fdir,
                active = row.active
            }).Cast<SETG>().ToList();
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETG();
            Focus();
        }

        private void setSETL()
        {
            dgvSETL.DataSource = playbookFormRef.Play.SETL;

            dgvSETL.Columns["rec"].ReadOnly = true;
            dgvSETL.Columns["SETL"].ReadOnly = true;

            dgvSETL.AutoResizeColumns();
            Program.ResizeDataGrid(dgvSETL);

            foreach (SETL setl in playbookFormRef.Play.SETL)
            {
                Console.WriteLine(setl);
            }
        }

        private void dgvSETL_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dgvSETL.AutoResizeColumns();
            Program.ResizeDataGrid(dgvSETL);
            int SETLrec = ((SETL)playbookFormRef.cbxSubFormations.SelectedItem).rec;
            int PBPLrec = ((PBPL)playbookFormRef.cbxPlays.SelectedItem).rec;

            playbookFormRef.SetPlay(playbookFormRef.Play);
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdateSubFormations();
            playbookFormRef.UpdateAlignments();
            playbookFormRef.UpdatePlays();

            playbookFormRef.cbxSubFormations.SelectedItem = (playbookFormRef.cbxSubFormations.Items.Cast<SETL>().ToList()).Find(item => item.rec == SETLrec);
            playbookFormRef.cbxPlays.SelectedItem = (playbookFormRef.cbxPlays.Items.Cast<PBPL>().ToList()).Find(item => item.rec == PBPLrec);

            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETL();

            Focus();
        }

        private void setSETP()
        {
            dgvSETP.DataSource = playbookFormRef.Play.SETP;

            dgvSETP.Columns["rec"].ReadOnly = true;
            dgvSETP.Columns["SETL"].ReadOnly = true;
            dgvSETP.Columns["SETP"].ReadOnly = true;
            dgvSETP.Columns["poso"].ReadOnly = true;

            dgvSETP.AutoResizeColumns();
            Program.ResizeDataGrid(dgvSETP);

            foreach (SETP setp in playbookFormRef.Play.SETP)
            {
                Console.WriteLine(setp);
            }
        }

        private void dgvSETP_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            playbookFormRef.SetPlay(playbookFormRef.Play);
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETP();

            Program.ResizeDataGrid(dgvSETP);
            Focus();
        }

        private void setSETG()
        {
            dgvSETG.DataSource = playbookFormRef.Play.SETG;

            dgvSETG.Columns["active"].Visible = false;
            dgvSETG.Columns["rec"].ReadOnly = true;
            dgvSETG.Columns["rec"].DefaultCellStyle.BackColor = Color.White;
            dgvSETG.Columns["rec"].DefaultCellStyle.SelectionBackColor = Color.White;
            dgvSETG.Columns["rec"].DefaultCellStyle.ForeColor = Color.DarkGray;
            dgvSETG.Columns["rec"].DefaultCellStyle.SelectionForeColor = Color.DarkGray;
            dgvSETG.Columns["SETP"].ReadOnly = true;
            dgvSETG.Columns["SETP"].ReadOnly = true;
            dgvSETG.Columns["SETP"].DefaultCellStyle.BackColor = Color.White;
            dgvSETG.Columns["SETP"].DefaultCellStyle.SelectionBackColor = Color.White;
            dgvSETG.Columns["SETP"].DefaultCellStyle.ForeColor = Color.DarkGray;
            dgvSETG.Columns["SETP"].DefaultCellStyle.SelectionForeColor = Color.DarkGray;
            dgvSETG.Columns["SGF_"].ReadOnly = true;
            dgvSETG.Columns["SGF_"].ReadOnly = true;
            dgvSETG.Columns["SGF_"].DefaultCellStyle.BackColor = Color.White;
            dgvSETG.Columns["SGF_"].DefaultCellStyle.SelectionBackColor = Color.White;
            dgvSETG.Columns["SGF_"].DefaultCellStyle.ForeColor = Color.DarkGray;
            dgvSETG.Columns["SGF_"].DefaultCellStyle.SelectionForeColor = Color.DarkGray;

            dgvSETG.AutoResizeColumns();
            Program.ResizeDataGrid(dgvSETG);

            for (int i = 0; i < playbookFormRef.Play.SETG.Count; i++)
            {
                if (!(playbookFormRef.Play.SETG[i].active))
                {
                    dgvSETG.Rows[i].ReadOnly = true;
                    dgvSETG.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dgvSETG.Rows[i].DefaultCellStyle.SelectionBackColor = Color.White;
                    dgvSETG.Rows[i].DefaultCellStyle.ForeColor = Color.DarkGray;
                    dgvSETG.Rows[i].DefaultCellStyle.SelectionForeColor = Color.DarkGray;
                }
            }

            foreach (SETG setg in playbookFormRef.Play.SETG)
            {
                Console.WriteLine(setg);
            }
        }

        private void dgvSETG_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            playbookFormRef.SetPlay(playbookFormRef.Play);
            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETG();

            Program.ResizeDataGrid(dgvSETG);
            Focus();
        }

        private void dgvSETP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (PSALroute poso in playbookFormRef.Play.posos)
            {
                poso.Highlighted = false;
            }

            playbookFormRef.Play.posos[dgvSETP.CurrentRow.Index].Highlighted = true;

            playbookFormRef.ClearField();
            playbookFormRef.DrawPlayartPSAL(Graphics.FromImage(playbookFormRef.gImage), playbookFormRef.Play);
        }

        private void dgvSETG_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (PSALroute poso in playbookFormRef.Play.posos)
            {
                poso.Highlighted = false;
            }

            playbookFormRef.Play.posos[dgvSETG.CurrentRow.Index].Highlighted = true;

            playbookFormRef.ClearField();
            playbookFormRef.DrawPlayartPSAL(Graphics.FromImage(playbookFormRef.gImage), playbookFormRef.Play);
        }

        private void editSETGadd(object sender, EventArgs e)
        {
            int SGF_ = ((SGFF)playbookFormRef.cbxAlignments.SelectedItem).SGF_;

            SETG setg = new SETG();

            setg.rec = playbookFormRef.SETG.Count;
            setg.SETP = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].SETP;
            setg.SGF_ = SGF_;
            setg.SF__ = 0;
            setg.x___ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].x___;
            setg.y___ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].y___;
            setg.fx__ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].fx__;
            setg.fy__ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].fy__;
            setg.anm_ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].anm_;
            setg.dir_ = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].dir_;
            setg.fanm = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].fanm;
            setg.fdir = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].fdir;

            playbookFormRef.SETG.Add(setg);

            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETG();
        }

        private void editSETGremove(object sender, EventArgs e)
        {
            int record = playbookFormRef.Play.SETG[dgvSETG.CurrentCell.RowIndex].rec;

            for (int i = playbookFormRef.SETG.Count - 1; i > record; i--)
            {
                playbookFormRef.SETG[i].rec -= 1;
            }

            playbookFormRef.SETG.RemoveAt(record);

            playbookFormRef.Play = playbookFormRef.GetPlay();
            playbookFormRef.UpdatePlay(playbookFormRef.Play);
            setSETG();
        }

        private void dgvSETG_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            dgvSETG.CurrentCell = dgvSETG[e.ColumnIndex, e.RowIndex];

            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                if (((SGFF)playbookFormRef.cbxAlignments.SelectedItem).name == "Norm")
                {
                    cmsAddPlayerToMotion.Enabled = false;
                    cmsRemovePlayerFromMotion.Enabled = false;
                }
                else
                {
                    if (playbookFormRef.Play.SETG[e.RowIndex].active)
                    {
                        cmsAddPlayerToMotion.Enabled = false;
                        cmsRemovePlayerFromMotion.Enabled = true;
                    }
                    else
                    {
                        cmsAddPlayerToMotion.Enabled = true;
                        cmsRemovePlayerFromMotion.Enabled = false;
                    }
                }

                cmsEditSETG.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }
    }
}
