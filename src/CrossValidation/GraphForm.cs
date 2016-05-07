using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpNeatGUI;
using ZedGraph;

namespace CrossValidation
{
    public partial class GraphForm : Form
    {
        private List<RollingPointPairList> plotList = new List<RollingPointPairList>(); 
        private GraphPane _graphPane;
        private List<List<SampleRow>> datasource = new List<List<SampleRow>>(); 
        private List<Color> colors = new List<Color>();
        private Color[] defaults = new[] {Color.BlueViolet, Color.Blue, Color.ForestGreen, Color.MediumVioletRed,
            Color.Brown, Color.DeepPink, Color.Black};

        public GraphForm()
        {
            InitializeComponent();
            _graphPane = zed.GraphPane;
            _graphPane.IsBoundedRanges = false;

            _graphPane.Title.Text = string.Empty;

            _graphPane.XAxis.Title.Text = "Generations count";
            _graphPane.XAxis.MajorGrid.IsVisible = true;

            _graphPane.YAxis.Title.Text = "Best fitness";
            _graphPane.YAxis.MajorGrid.IsVisible = true;

            _graphPane.Y2Axis.Title.Text = string.Empty;
            _graphPane.Y2Axis.MajorGrid.IsVisible = false;


        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Have the user choose the genome XML file.
            var result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
                return;
            string[] data = File.ReadAllLines(openFileDialog1.FileName);
            List<SampleRow> sample = parse(data);
            datasource.Add(sample);
            int sourceCount = Int32.Parse(textBox1.Text);
            int bound = sourceCount > sample.Count ? sample.Count : sourceCount;

            RollingPointPairList pointPlotArray = new RollingPointPairList(bound);
            LineItem lineItem = _graphPane.AddCurve(string.Empty, pointPlotArray, defaults[datasource.Count - 1], SymbolType.None);
            lineItem.Line.Width = 3f;
            lineItem.IsY2Axis = false;
            plotList.Add(pointPlotArray);
            for (int i = 0; i < bound; i++)
            {
                pointPlotArray.Add(sample[i].testX, sample[i].testY);
            }
            // Trigger graph to redraw.
            zed.AxisChange();
            zed.Refresh();
        }

        private List<SampleRow> parse(string[] data)
        {
            List<SampleRow> result = new List<SampleRow>();
            for (int i = 0; i < data.Length; i++)
            {
                string[] row = data[i].Split(' ');
                SampleRow sr = new SampleRow();
                for (int j = 0; j < row.Length; j++)
                {
                    double value = GetDouble(row[j]);
                    if (j == 0) sr.trainX = value;
                    if (j == 1) sr.trainY = value;
                    if (j == 2) sr.testX = value;
                    if (j == 3) sr.testY = value;
                }
                result.Add(sr);
            }
            return result;
        }

        protected double GetDouble(string value)
        {
            double result;
            if (value.Contains(","))
            {
                double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
                return result;
            }
            double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result);     
            return result;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void clear()
        {
            foreach (var pointPairs in plotList)
            {
                pointPairs.Clear();
            }

            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        private void reset()
        {
            plotList = new List<RollingPointPairList>();
            datasource = new List<List<SampleRow>>();
            clear();
        }

        private void redraw()
        {
            clear();

            foreach (var data in datasource)
            {
                
            }
            // Trigger graph to redraw.
            zed.AxisChange();
            Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear();
            PictureBox pb = new PictureBox();
        }

        class SampleRow
        {
            public double trainX;
            public double trainY;
            public double testX;
            public double testY;
        }
    }
}
