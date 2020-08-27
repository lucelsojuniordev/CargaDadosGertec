using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CargaDadosGertec
{
    static class Program
    {
      
        [STAThread]
        static void Main()
        {
            var instancias = 0;

            var nomeProcesso = Process.GetCurrentProcess().ProcessName;

            nomeProcesso = nomeProcesso.Replace(".vshost", "").Replace(".exe", "");

            instancias = Process.GetProcesses().Count(process => process.ProcessName.Contains(nomeProcesso));

            //Se já houver um aberto
            if (instancias > 1)
            {
                //Mostra mensagem de erro e finaliza
                MessageBox.Show(string.Format("Já existe {0} em funcionamento neste computador!", nomeProcesso), "Atenção - Programa em Execução", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
