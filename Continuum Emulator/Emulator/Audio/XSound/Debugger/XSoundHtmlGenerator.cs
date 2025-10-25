using System;
using System.IO;
using System.Text;

namespace Continuum93.Emulator.Audio.XSound.Debug
{
    public static class XSoundHtmlGenerator
    {
        public static void GenerateHtmlWithSoundData(float[] soundData, string filePath)
        {
            // Convert soundData array to a JavaScript-compatible array string
            StringBuilder soundDataString = new StringBuilder();
            soundDataString.Append("[");
            for (int i = 0; i < soundData.Length; i++)
            {
                soundDataString.Append(soundData[i].ToString("G", System.Globalization.CultureInfo.InvariantCulture)); // Use invariant culture to format numbers correctly
                if (i < soundData.Length - 1)
                {
                    soundDataString.Append(",");
                }
            }
            soundDataString.Append("]");

            // Define the HTML template
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <script src='https://hammerjs.github.io/dist/hammer.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/chartjs-plugin-zoom@1.0.0'></script>
    <title>Waveform Plot</title>
</head>
<body>
    <canvas id='waveformChart' width='800' height='400'></canvas>

    <script>
        var ctx = document.getElementById('waveformChart').getContext('2d');

        // Inserted sound data
        var soundData = " + soundDataString.ToString() + @";
        var timeData = soundData.map((v, i) => i); // X-axis is sample index

        // Plugin to draw a horizontal line at y = 0
        const horizontalLinePlugin = {
            id: 'horizontalLinePlugin',
            afterDraw: function(chart) {
                /*if (chart.tooltip._active && chart.tooltip._active.length) {
                    return;
                }*/
                var ctx = chart.ctx;
                var yScale = chart.scales.y;
                var yValue = yScale.getPixelForValue(0); // Get pixel position for y = 0

                ctx.save();
                ctx.beginPath();
                ctx.moveTo(chart.scales.x.left, yValue);
                ctx.lineTo(chart.scales.x.right, yValue);
                ctx.lineWidth = 1;
                ctx.strokeStyle = 'rgba(0, 0, 0, 0.7)'; // Color for the line
                ctx.stroke();
                ctx.restore();
            }
        };


        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: timeData,
                datasets: [{
                    label: 'Amplitude',
                    data: soundData,
                    borderColor: 'rgba(0, 0, 0, 1)',
                    fill: false
                }]
            },
            options: {
                responsive: true,
                scales: {
                    x: { title: { display: true, text: 'Sample Index' } },
                    y: { title: { display: true, text: 'Amplitude' } }
                },
                plugins: {
                    zoom: {
                        pan: {
                            enabled: true,
                            mode: 'x',
                        },
                        zoom: {
                            mode: 'x',
							wheel: {enabled: true},
							drag: {enabled: false}
						}
                    }
                }
            },
            plugins: [horizontalLinePlugin] // Add the horizontal line plugin
        });
    </script>
</body>
</html>";

            // Save the HTML file
            File.WriteAllText(filePath, htmlTemplate);
            Console.WriteLine("HTML file generated at: " + filePath);
        }
    }
}
