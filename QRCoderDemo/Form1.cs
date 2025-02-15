using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;


namespace QRCoderDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxECC.SelectedIndex = 0; //Pre-select ECC level "L"

            PayloadGenerator.RussiaPaymentOrder.MandatoryFields mFld = new PayloadGenerator.RussiaPaymentOrder.MandatoryFields()
            {
                PersonalAcc = "40702810138250123017",
                BIC = "044525225",
                BankName = "��� \"����\"",
                Name = "��� ���� ����",
                CorrespAcc = "30101810965770000413"
            };

            PayloadGenerator.RussiaPaymentOrder.OptionalFields oFld = new PayloadGenerator.RussiaPaymentOrder.OptionalFields()
            {
                Sum = "456"
            };
            PayloadGenerator.RussiaPaymentOrder.OptionalExtFields oExtFld = new PayloadGenerator.RussiaPaymentOrder.OptionalExtFields()
            {
                FirstName = "test"
            };
//            var generator = new PayloadGenerator.RussiaPaymentOrder(mFld, PayloadGenerator.RussiaPaymentOrder.CharacterSets.utf_8);
//            var generator = new PayloadGenerator.RussiaPaymentOrder(mFld, oFld, PayloadGenerator.RussiaPaymentOrder.CharacterSets.utf_8);
            var generator = new PayloadGenerator.RussiaPaymentOrder(mFld, oFld, oExtFld, PayloadGenerator.RussiaPaymentOrder.CharacterSets.utf_8);

            string payload = generator.ToString();
            textBoxQRCode.Text = payload;




            RenderQrCode();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            RenderQrCode();
        }

        private void RenderQrCode()
        {
            string level = comboBoxECC.SelectedItem.ToString();
            QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBoxQRCode.Text, eccLevel))

                /*
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                pictureBoxQRCode.BackgroundImage = qrCode.GetGraphic(20, GetPrimaryColor(), GetBackgroundColor(),
                    GetIconBitmap(), (int)iconSize.Value);

                this.pictureBoxQRCode.Size = new System.Drawing.Size(pictureBoxQRCode.Width, pictureBoxQRCode.Height);
                //Set the SizeMode to center the image.
                this.pictureBoxQRCode.SizeMode = PictureBoxSizeMode.CenterImage;

                pictureBoxQRCode.SizeMode = PictureBoxSizeMode.StretchImage;
            }
                */
            using (ArtQRCode qrCode = new ArtQRCode(qrCodeData))
            {
               // pictureBoxQRCode.BackgroundImage = qrCode.GetGraphic(20);//, GetPrimaryColor(), GetBackgroundColor(),true);//',
                                                                         //  GetIconBitmap(), (int)iconSize.Value);
                pictureBoxQRCode.BackgroundImage = qrCode.GetGraphic(
                    20,
                    GetPrimaryColor(),
                    GetBackgroundColor(), 
                    Color.White, 
                    null,
                    .5, 
                    true, 
                    ArtQRCode.QuietZoneStyle..Dotted, 
                    ArtQRCode.BackgroundImageStyle.DataAreaOnly,
                    null);

                this.pictureBoxQRCode.Size = new System.Drawing.Size(pictureBoxQRCode.Width, pictureBoxQRCode.Height);
                //Set the SizeMode to center the image.
                this.pictureBoxQRCode.SizeMode = PictureBoxSizeMode.CenterImage;

                pictureBoxQRCode.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            }

            private Bitmap GetIconBitmap()
        {
            if (iconPath.Text.Length == 0)
            {
                return null;
            }
            try
            {
                return new Bitmap(iconPath.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void selectIconBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "Select icon";
            openFileDlg.Multiselect = false;
            openFileDlg.CheckFileExists = true;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                iconPath.Text = openFileDlg.FileName;
                if (iconSize.Value == 0)
                {
                    iconSize.Value = 15;
                }
            }
            else
            {
                iconPath.Text = "";
            }
        }


        private void btn_save_Click(object sender, EventArgs e)
        {

            // Displays a SaveFileDialog so the user can save the Image
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp|PNG Image|*.png|JPeg Image|*.jpg|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                using (FileStream fs = (System.IO.FileStream) saveFileDialog1.OpenFile())
                {
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.

                    ImageFormat imageFormat = null;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            imageFormat = ImageFormat.Bmp;
                            break;
                        case 2:
                            imageFormat = ImageFormat.Png;
                            break;
                        case 3:
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case 4:
                            imageFormat = ImageFormat.Gif;
                            break;
                        default:
                            throw new NotSupportedException("File extension is not supported");
                    }

                    pictureBoxQRCode.BackgroundImage.Save(fs, imageFormat);
                }
            }
        }

        public void ExportToBmp(string path)
        {

        }

        private void textBoxQRCode_TextChanged(object sender, EventArgs e)
        {
            RenderQrCode();
        }

        private void comboBoxECC_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderQrCode();
        }

        private void panelPreviewPrimaryColor_Click(object sender, EventArgs e)
        {
            if (colorDialogPrimaryColor.ShowDialog() == DialogResult.OK)
            {
                panelPreviewPrimaryColor.BackColor = colorDialogPrimaryColor.Color;
                RenderQrCode();
            }
        }

        private void panelPreviewBackgroundColor_Click(object sender, EventArgs e)
        {
            if (colorDialogBackgroundColor.ShowDialog() == DialogResult.OK)
            {
                panelPreviewBackgroundColor.BackColor = colorDialogBackgroundColor.Color;
                RenderQrCode();
            }
        }

        private Color GetPrimaryColor()
        {
            return panelPreviewPrimaryColor.BackColor;
        }

        private Color GetBackgroundColor()
        {
            return panelPreviewBackgroundColor.BackColor;
        }
    }
}
