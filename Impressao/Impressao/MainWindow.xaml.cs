using System;
using System.Drawing.Printing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Impressao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string arquivo { get; set; }

        public bool ModoTeste { get; set; } = false;

        public bool CarteirinhaLiga { get; set; } = false;

        private string LogMsg;

        public bool FotoBaixado { get; set; }
        public bool EscudoBaixado { get; set; }

        // Defini modo de impressão em retrato ou paisagem      
        // Algumas impressoras aceitam em retrato e outras como paisagem
        private bool _modoRetrato { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this._modoRetrato = true;
        }

        private void IniciouImpressao(object sender, PrintEventArgs e)
        {
            lblStatus.Visibility = Visibility.Visible;
            lblStatus.Content = "Aguarde....";
        }
        private void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            lblStatus.Content = "Enviado para impressora";
        }
        private void PrintPage(object o, PrintPageEventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(arquivo);

            //Retrato
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, 220, 341);

            //Paisagem
            //System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, 341, 220);
            e.Graphics.DrawImage(img, rec);
        }
        private void SalvarArquivo()
        {

            int x = Convert.ToInt32(txtX.Text);

            int y = Convert.ToInt32(txtY.Text);

            int renderX = Convert.ToInt32(txtRenderX.Text);
            int renderY = Convert.ToInt32(txtRenderY.Text);

            arquivo = System.IO.Path.GetTempPath() + "1.bmp";

            if (_modoRetrato)
            {
                rotacionar.Angle = 90;
                rotacionar.CenterX = x;
                rotacionar.CenterY = y;
            }


            int Height = (int)this.carteirinha.ActualWidth;
            int Width = (int)this.carteirinha.ActualHeight;


            RenderTargetBitmap bmp = RenderVisual(carteirinha, Width, Height);


            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }
            string Extension = System.IO.Path.GetExtension(arquivo).ToLower();

            BitmapEncoder encoder;
            if (Extension == ".gif")
                encoder = new GifBitmapEncoder();
            else if (Extension == ".png")
                encoder = new PngBitmapEncoder();
            else if (Extension == ".jpg")
                encoder = new JpegBitmapEncoder();
            else if (Extension == ".bmp")
                encoder = new BmpBitmapEncoder();
            else
                return;

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stm = File.Create(arquivo))
            {
                encoder.Save(stm);
            }
        }
        public RenderTargetBitmap RenderVisual(UIElement elt, int width, int height)
        {
            PresentationSource source = PresentationSource.FromVisual(elt);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)width,
                  (int)height, 96, 96, PixelFormats.Default);

            VisualBrush sourceBrush = new VisualBrush(elt);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
                      new Point(width, height)));
            }
            rtb.Render(drawingVisual);

            return rtb;
        }
        private void btImprimeDireto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SalvarArquivo();
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintPage;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FotoEscudo_Changed(object sender, EventArgs e)
        {
            EscudoBaixado = true;
            if (EscudoBaixado && FotoBaixado)
            {
                btImprimeAssist.Visibility = Visibility.Visible;
                btImprimeDireto.Visibility = Visibility.Visible;
                lblStatus.Content = "Carregado";
            }
        }

        private void FotoAtl_Changed(object sender, EventArgs e)
        {
            FotoBaixado = true;
            if (EscudoBaixado && FotoBaixado)
            {
                btImprimeAssist.Visibility = Visibility.Visible;
                btImprimeDireto.Visibility = Visibility.Visible;
                lblStatus.Content = "Carregado";
            }
        }

        private void FotoEscudo_DownloadCompleted(object sender, EventArgs e)
        {
            EscudoBaixado = true;
            if (EscudoBaixado && FotoBaixado)
            {
                btImprimeAssist.Visibility = Visibility.Visible;
                btImprimeDireto.Visibility = Visibility.Visible;
                lblStatus.Content = "Carregado";
            }
        }

        private void FotoAtl_DownloadCompleted(object sender, EventArgs e)
        {
            FotoBaixado = true;
            if (EscudoBaixado && FotoBaixado)
            {
                btImprimeAssist.Visibility = Visibility.Visible;
                btImprimeDireto.Visibility = Visibility.Visible;
                lblStatus.Content = "Carregado";
            }
        }

        private void btImprimeAssist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SalvarArquivo();

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintPage;
                pd.EndPrint += printDoc_EndPrint;
                pd.BeginPrint += IniciouImpressao;

                System.Windows.Forms.PrintDialog pdi = new System.Windows.Forms.PrintDialog();
                pdi.Document = pd;
                if (pdi.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pd.Print();
                }
                else
                {
                    MessageBox.Show("Impressão Cancelada");
                }
            }
            catch (Exception ex)

            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CarregaCarteirinhasDefaults()
        {
            lblNomeClube.Content = "";
            lblNomeAtleta.Content = "";
            lblNascimento.Content = "";
            lblCPF.Content = "";
            fotoAtleta.Source = null;
            imgLogoClube.Source = null;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
