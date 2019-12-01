using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace WinHash_Free
{
    public partial class Form1 : Form
    {
#pragma warning disable IDE0069 // Campos descartáveis devem ser descartados
        private readonly MD5 Md5 = MD5.Create();
#pragma warning restore IDE0069 // Campos descartáveis devem ser descartados
        private readonly SHA1 Sha1 = SHA1.Create();
#pragma warning disable IDE0069 // Campos descartáveis devem ser descartados
        private readonly SHA256 Sha3 = SHA256.Create();
#pragma warning restore IDE0069 // Campos descartáveis devem ser descartados

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // label logo
            lbLogo.Location = new Point(85, 25);
            lbLogo.Parent = pbLogo;
            lbLogo.BackColor = Color.Transparent;
            // hash check button
            btnVerify.Enabled = false;
            // groupBox
            groupBox2.Enabled = true;
            groupBox3.Enabled = false;
            // Save Hash
            btnSaveHash.Enabled = false;
        }

        private void btnSearchFile_Click(object sender, EventArgs e)
        {
            ofdlg1.FileName = txtFile.Text;
            if(ofdlg1.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = ofdlg1.FileName;
                txtMD5.Clear();
                txtSHA1.Clear();
                txtSHA3.Clear();

                // Calculates the size of the selected file
                long fileSize = new FileInfo(ofdlg1.FileName).Length;
                lbSize.Text = FormatDisplaySizeFile(fileSize);
                // Returns the filename and string extension of the specified path
                string result = Path.GetFileName(ofdlg1.FileName);
                lbFileName.Text = result;
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (txtHash.Text == string.Empty)
            {
                btnVerify.Enabled = false;
            }
            else
            {
                btnVerify.Enabled = true;
                
                if (txtFile.Text == string.Empty)
                {
                    MessageBox.Show("Você deve selecionar um arquivo para poder prosseguir!", "AVISO!");
                }
                else
                {
                    if(rbMD5.Checked == true)
                    {
                        txtMD5.Text = BytesToString(GetHashMD5(txtFile.Text));
                        if(txtMD5.Text == txtHash.Text)
                        {
                            ShowMessage(1);
                        }
                        else
                        {
                            ShowMessage(2);
                        }
                    }
                    else if (rbSHA1.Checked == true)
                    {
                        txtSHA1.Text = BytesToString(GetHashSHA1(txtFile.Text));
                        if (txtSHA1.Text == txtHash.Text)
                        {
                            ShowMessage(1);
                        }
                        else
                        {
                            ShowMessage(2);
                        }
                    }
                    else
                    {
                        txtSHA3.Text = BytesToString(GetHashSHA3(txtFile.Text));
                        if (txtSHA3.Text == txtHash.Text)
                        {
                            ShowMessage(1);
                        }
                        else
                        {
                            ShowMessage(2);
                        }
                    }
                }                
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if(txtFile.Text == string.Empty)
            {
                MessageBox.Show("Você deve selecionar um arquivo para poder prosseguir!", "AVISO!");
            }
            else
            {
                txtMD5.Text = BytesToString(GetHashMD5(txtFile.Text));
                txtSHA1.Text = BytesToString(GetHashSHA1(txtFile.Text));
                txtSHA3.Text = BytesToString(GetHashSHA3(txtFile.Text));

                btnSaveHash.Enabled = true;
            }
        }
        
        private byte[] GetHashMD5(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                return Md5.ComputeHash(stream);
            }
        }

        private byte[] GetHashSHA1(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                return Sha1.ComputeHash(stream);
            }
        }
        private byte[] GetHashSHA3(string file)
        {
            using(FileStream stream = File.OpenRead(file))
            {
                return Sha3.ComputeHash(stream);
            }
        }

        public static string BytesToString(byte[] bytes)
        {
            string resultado = "";
            foreach(byte b in bytes)
            {
                resultado += b.ToString("x2"); 
            }
            return resultado;
        }

        private void rbVerifyHash_CheckedChanged(object sender, EventArgs e)
        {
            if (rbVerifyHash.Checked == true)
            {
                groupBox2.Enabled = false;
                groupBox3.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = true;
                groupBox3.Enabled = false;
                txtHash.Clear();
            }
        }

        // Returns the file size to a size.
        // The default format is "0.### XB", Ex: "4.2 KB" or "1.434 GB"
        public string FormatDisplaySizeFile(long i)
        {
            // Gets the absolute value
            long i_absolute = (i < 0 ? -i : i);
            // Determines the suffix and the value
            string suffix;
            double reading;
            if (i_absolute >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                reading = (i >> 50);
            }
            else if (i_absolute >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                reading = (i >> 40);
            }
            else if (i_absolute >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                reading = (i >> 30);
            }
            else if (i_absolute >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                reading = (i >> 20);
            }
            else if (i_absolute >= 0x100000) // Megabyte
            {
                suffix = "MB";
                reading = (i >> 10);
            }
            else if (i_absolute >= 0x400) // Kilobyte
            {
                suffix = "KB";
                reading = i;
            }
            else
            {
                return i.ToString("0 bytes"); // Byte
            }
            // Divide by 1024 to get the fractional value
            reading = (reading / 1024);
            // Returns the suffix formatted number
            return reading.ToString("0.## ") + suffix;
        }

        // Writes the generated hash to an MD5 file
        public static void writeHashMD5(string _content)
        {
            if (_content != string.Empty)
            {
                string _general = "hash.md5";

                FileStream fs = new FileStream(_general, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(_content);
                sw.Flush();
                sw.Close();
                fs.Close();
                // Exibe uma mensagem informando que os dados foram gravados.
                MessageBox.Show("Arquivo MD5 gravado com sucesso!", "AVISO!");
            }
        }

        // Writes the generated hash to an SHA1 file
        public static void writeHashSHA1(string _content)
        {
            if (_content != string.Empty)
            {
                string _general = "hash.sha1";

                FileStream fs = new FileStream(_general, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(_content);
                sw.Flush();
                sw.Close();
                fs.Close();
                // Exibe uma mensagem informando que os dados foram gravados.
                MessageBox.Show("Arquivo SHA-1 (160) gravado com sucesso!", "AVISO!");
            }
        }

        // Writes the generated hash to an SHA3 file
        public static void writeHashSHA3(string _content)
        {
            if (_content != string.Empty)
            {
                string _general = "hash.sha3";

                FileStream fs = new FileStream(_general, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(_content);
                sw.Flush();
                sw.Close();
                fs.Close();
                // Exibe uma mensagem informando que os dados foram gravados.
                MessageBox.Show("Arquivo SHA-3 (256) gravado com sucesso!", "AVISO!");
            }
        }

        private int ShowMessage(int _message)
        {
            if (_message == 1)
            {
                MessageBox.Show("Os valores são IGUAIS!", "AVISO!");
            }
            else if (_message == 2)
            {
                MessageBox.Show("Os valores são DIFERENTES!", "AVISO!");
            }
            else
            {
                MessageBox.Show("Arquivos Hash's gravados com sucesso!", "AVISO!");
            }
            return _message;
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox _about = new AboutBox(); 
            _about.ShowDialog();
        }

        private void btnSaveHash_Click(object sender, EventArgs e)
        {
                writeHashMD5(txtMD5.Text);
                writeHashSHA1(txtSHA1.Text);
                writeHashSHA3(txtSHA3.Text);
        }

        private void txtHash_TextChanged(object sender, EventArgs e)
        {
            if(txtHash.Text != string.Empty)
            {
                btnVerify.Enabled = true;
            }
            else
            {
                btnVerify.Enabled = false;
            }
        }
    }
}
