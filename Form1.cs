using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CargaDadosGertec
{
    public partial class Form1 : Form
    {
        public FbConnection conn { get; set; }
        private bool allowVisible;
        private bool allowClose;

        public Form1()
        {
            InitializeComponent();
            Executado();
            cargaGertec.Icon = new Icon(GetType(), "tool_120757.ico");            
        }

        public void Executado() {

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Program.als"))
            {
                MessageBox.Show("Arquivo Program.als não encontrado");
                return;
            }

            string[] arq = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Program.als").ToString().Split(':');
            string stringConexao = String.Format(@"User=analistats;Password=wvaebxti;Database={0};DataSource={1};Pooling=False;Connection timeout=25", arq[1] + ":" + arq[2].Split(';')[0], arq[0]);
            conn = new FbConnection { ConnectionString = stringConexao };
            StartarContador();
            Gerar();

        }

        private void Tempo_Tick(object sender, EventArgs e)
        {
            Gerar();
        }

        public void Gerar()
        {
            string sql = @"SELECT I.codigobarras || '|' || I.descricao || '|' || CAST(CAST(P.preco as numeric(10, 2)) AS VARCHAR(10))
                         FROM ITENS I INNER JOIN produtospreco P ON I.item = P.item AND P.tabelapreco = 0
                         WHERE I.codigobarras IS not NULL AND replace(I.codigobarras, ' ', '') <> ' ' ";

            FbCommand cmd = new FbCommand(sql, conn);
            DataTable datatable = RetornaDT(cmd);
            File.Delete(AppDomain.CurrentDomain.BaseDirectory.ToString() + "PRODUTOS.txt");
            using (StreamWriter file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory.ToString() + "PRODUTOS.txt"))
            {
                foreach (DataRow dr in datatable.Rows)
                {
                    file.WriteLine(dr.ItemArray[0]);

                }
            }

            conn.Close();
        }

        public void StartarContador() 
        {
            timer1.Start();
            timer1.Interval = 60000;
            timer1.Enabled = true;
            timer1.Tick += Tempo_Tick;
        }

        private DataTable RetornaDT(FbCommand cmdComando)
        {
            FbDataAdapter daAdaptador = new FbDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmdComando.Connection = conn;
                daAdaptador.SelectCommand = cmdComando;
                daAdaptador.Fill(dt);
                return dt;
            }
            catch (FbException ex)
            {
                throw ex;
            }

        }

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                this.Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

    }
}
