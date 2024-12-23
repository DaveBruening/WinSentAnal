using Azure;
using Azure.AI.TextAnalytics;
namespace WinSentAnal {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e) {
            //textBox1.Text = "The chilidog tasted like dog food."; Neutral
            textBox1.Text = "The chilidog tasted heavenly.  The tofu burger tasted like barf.";
        }
        private async void button1_Click(object sender, EventArgs e) {
            string langKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
            string ept = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
            AzureKeyCredential akc = new AzureKeyCredential(langKey);
            Uri uri = new Uri(ept);
            TextAnalyticsClient tac = new TextAnalyticsClient(uri, akc);
            Response<DocumentSentiment> rds = await tac.AnalyzeSentimentAsync(textBox1.Text);
            //MemoryStream ms = (MemoryStream)rds.GetRawResponse().ContentStream;
            //StreamReader srd = rds.Value;
            DocumentSentiment ds = rds.Value;
            string nl = Environment.NewLine;
            textBox2.Text = "Sentiment: " + ds.Sentiment.ToString() + "   " +
                $"+: {ds.ConfidenceScores.Positive}   -: {ds.ConfidenceScores.Negative}   "
                + $"neutral: {ds.ConfidenceScores.Neutral}";  
            int cntSntc=1;
            foreach (SentenceSentiment ss in ds.Sentences) {
                SentimentConfidenceScores scs = ss.ConfidenceScores;
                textBox2.Text += $"{nl}\tsentence#{cntSntc++}.: {ss.Text}   {ss.Sentiment}  " +
                    $"+: {scs.Positive}   -: {scs.Negative}   neutral: {scs.Neutral}";
            }  
            textBox2.Text += nl + "ReasonPhrase" + rds.GetRawResponse().ReasonPhrase;    //OK
            textBox2.Text += nl + "Header count: " +    //10
                rds.GetRawResponse().Headers.Count().ToString();
            foreach (Azure.Core.HttpHeader rh in rds.GetRawResponse().Headers)
                textBox2.Text += $"{nl}\tname: {rh.Name}    value: {rh.Value}";
            /* 'Unable to cast object of type 'System.IO.ReadOnlyMemoryStream' to type 
             'System.IO.FileStream':
            FileStream fs = (FileStream)rds.GetRawResponse().Content.ToStream();
            //public System.IO.ReadOnlyMemoryStream roms = rds.GetRawResponse().Content.ToStream();
            byte[] ba = new byte[1000];
            fs.Read(ba,0,ba.Length); */
            textBox2.Text += nl + "ClientRequestId: " + rds.GetRawResponse().ClientRequestId +
                $"   status:{rds.GetRawResponse().Status}";
            MemoryStream ms = (MemoryStream)rds.GetRawResponse().ContentStream;
            textBox2.Text += $"{nl}ms.CanRead: {ms.CanRead}";
            byte[] ba = new byte[1000];
            textBox2.Text += $"{nl}# bytes written to byte array: {ms.Read(ba, 0, ba.Length)}";
            textBox2.Text += $"{nl}byte array length: {ba.GetLength(0)}";
            textBox2.Text += nl + System.Text.Encoding.UTF8.GetString(ba);
        }
    }
}
