using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Django
{
    public class Module
    {
        public void InjectDLL(string _proc, string _dll)
        {
            switch (Injector.DllInjector.GetInstance.Inject(_proc, _dll))
            {
                case Injector.InjectionResult.dllNotFound:
                    int num1 = (int)XtraMessageBox.Show("Could not find the dll! Could it have possibly been moved?", "Error: Dll Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                case Injector.InjectionResult.gameProcessNotFound:
                    int num2 = (int)XtraMessageBox.Show("Process does not exist! Make sure you are still running said application.", "Apllication Process Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                case Injector.InjectionResult.injectionFailed:
                    int num3 = (int)XtraMessageBox.Show("Injection failed!. This can happen for a number of reasons.\n\n" +
                        "1. The chosen dll could be corrupt.\n" +
                        "2. The selected process could have stopped working.\n" +
                        "3. ", "Injection Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                case Injector.InjectionResult.Success:
                    int num4 = (int)XtraMessageBox.Show("Successfully Injected The Dll!", "Injection Complete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
            }
        }
    }
}
