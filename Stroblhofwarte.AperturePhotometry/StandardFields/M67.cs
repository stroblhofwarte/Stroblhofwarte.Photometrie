#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* http://binaries.boulder.swri.edu/fields/m67.html, Arne's UBVRcIc data */
namespace Stroblhofwarte.AperturePhotometry.StandardFields
{
    public class M67
    {
        public List<StandardStar> Stars { get; set; }
        
        public M67() 
        {
            Stars = new List<StandardStar>()
            {
                new StandardStar() { Id = "01", Ra=132.799179, DEC=11.756206, V=10.016, BV=0.029, UB=-0.472, VR=-0.038, RI=- 0.031, Verr=0.018, BVerr=0.085, UBerr=0.074, VRerr=0.014, RIerr=0.018},
                new StandardStar() { Id = "02", Ra=132.821344, DEC=11.804549, V=10.289, BV=1.266, UB=1.358, VR=0.663, RI=0.562, Verr=0.015, BVerr=0.018, UBerr=0.031, VRerr=0.014, RIerr=0.015},
                new StandardStar() { Id = "03", Ra=132.845119, DEC=11.800557, V=10.453, BV=1.109, UB=1.018, VR=0.568, RI=0.498, Verr=0.012, BVerr=0.014, UBerr=0.026, VRerr=0.008, RIerr=0.011},
                new StandardStar() { Id = "04", Ra=132.861935, DEC=11.811315, V=10.489, BV=0.576, UB=0.051, VR=0.340, RI=0.327, Verr=0.013, BVerr=0.010, UBerr=0.017, VRerr=0.008, RIerr=0.013},
                new StandardStar() { Id = "05", Ra=132.802987, DEC=11.878504, V=10.526, BV=1.094, UB=1.000, VR=0.564, RI=0.489, Verr=0.014, BVerr=0.016, UBerr=0.025, VRerr=0.011, RIerr=0.010},
                new StandardStar() { Id = "06", Ra=132.870902, DEC=11.842597, V=10.534, BV=1.123, UB=1.043, VR=0.583, RI=0.513, Verr=0.012, BVerr=0.014, UBerr=0.025, VRerr=0.008, RIerr=0.010},
                new StandardStar() { Id = "07", Ra=132.931570, DEC=11.740761, V=10.762, BV=1.137, UB=1.103, VR=0.578, RI=0.528, Verr=0.016, BVerr=0.012, UBerr=0.024, VRerr=0.011, RIerr=0.012},
                new StandardStar() { Id = "08", Ra=132.809956, DEC=11.750341, V=10.899, BV=0.436, UB=0.062, VR=0.268, RI=0.252, Verr=0.025, BVerr=0.014, UBerr=0.019, VRerr=0.012, RIerr=0.014},
                new StandardStar() { Id = "09", Ra=132.893091, DEC=11.852981, V=10.927, BV=0.243, UB=0.112, VR=0.142, RI=0.154, Verr=0.013, BVerr=0.009, UBerr=0.018, VRerr=0.009, RIerr=0.010},
                new StandardStar() { Id = "10", Ra=132.862659, DEC=11.864667, V=10.946, BV=0.097, UB=0.073, VR=0.044, RI=0.056, Verr=0.017, BVerr=0.010, UBerr=0.020, VRerr=0.010, RIerr=0.011},
                new StandardStar() { Id = "11", Ra=132.885920, DEC=11.814540, V=11.063, BV=0.221, UB=0.131, VR=0.116, RI=0.127, Verr=0.017, BVerr=0.009, UBerr=0.019, VRerr=0.010, RIerr=0.012},
                new StandardStar() { Id = "12", Ra=132.821106, DEC=11.846304, V=11.132, BV=1.090, UB=0.968, VR=0.573, RI=0.499, Verr=0.014, BVerr=0.012, UBerr=0.025, VRerr=0.010, RIerr=0.013},
                new StandardStar() { Id = "13", Ra=132.860242, DEC=11.730831, V=11.262, BV=0.130, UB=0.082, VR=0.048, RI=0.070, Verr=0.015, BVerr=0.011, UBerr=0.019, VRerr=0.007, RIerr=0.014},
                new StandardStar() { Id = "14", Ra=132.926626, DEC=11.856478, V=11.267, BV=1.076, UB=0.962, VR=0.571, RI=0.508, Verr=0.011, BVerr=0.012, UBerr=0.028, VRerr=0.011, RIerr=0.013},
                new StandardStar() { Id = "15", Ra=132.840744, DEC=11.877244, V=11.306, BV=0.607, UB=0.146, VR=0.359, RI=0.335, Verr=0.013, BVerr=0.014, UBerr=0.017, VRerr=0.011, RIerr=0.009},
                new StandardStar() { Id = "16", Ra=132.764746, DEC=11.750831, V=11.313, BV=0.292, UB=0.121, VR=0.163, RI=0.162, Verr=0.017, BVerr=0.012, UBerr=0.019, VRerr=0.013, RIerr=0.014},
                new StandardStar() { Id = "17", Ra=132.839951, DEC=11.768455, V=11.427, BV=1.074, UB=0.960, VR=0.560, RI=0.492, Verr=0.014, BVerr=0.011, UBerr=0.026, VRerr=0.008, RIerr=0.012},
                new StandardStar() { Id = "18", Ra=132.849166, DEC=11.830434, V=11.485, BV=0.885, UB=0.519, VR=0.488, RI=0.440, Verr=0.024, BVerr=0.012, UBerr=0.020, VRerr=0.007, RIerr=0.011},
                new StandardStar() { Id = "19", Ra=132.937934, DEC=11.796177, V=11.494, BV=1.052, UB=0.901, VR=0.554, RI=0.499, Verr=0.011, BVerr=0.013, UBerr=0.022, VRerr=0.008, RIerr=0.010},
                new StandardStar() { Id = "20", Ra=132.782656, DEC=11.802645, V=11.544, BV=0.406, UB=-0.039, VR=0.251, RI=0.243, Verr=0.013, BVerr=0.010, UBerr=0.018, VRerr=0.009, RIerr=0.011},
                new StandardStar() { Id = "21", Ra=132.926532, DEC=11.835538, V=11.637, BV=1.050, UB=0.909, VR=0.556, RI=0.499, Verr=0.011, BVerr=0.013, UBerr=0.025, VRerr=0.010, RIerr=0.013},
                new StandardStar() { Id = "23", Ra=132.833043, DEC=11.783526, V=12.116, BV=0.457, UB=0.021, VR=0.281, RI=0.269, Verr=0.012, BVerr=0.009, UBerr=0.020, VRerr=0.008, RIerr=0.012},
                new StandardStar() { Id = "24", Ra=132.914222, DEC=11.862751, V=12.139, BV=1.000, UB=0.801, VR=0.537, RI=0.477, Verr=0.011, BVerr=0.012, UBerr=0.023, VRerr=0.013, RIerr=0.012},
                new StandardStar() { Id = "25", Ra=132.806796, DEC=11.843961, V=12.213, BV=0.671, UB=0.171, VR=0.384, RI=0.352, Verr=0.012, BVerr=0.011, UBerr=0.014, VRerr=0.008, RIerr=0.009},
                new StandardStar() { Id = "26", Ra=132.885851, DEC=11.844686, V=12.238, BV=0.249, UB=0.064, VR=0.145, RI=0.155, Verr=0.016, BVerr=0.009, UBerr=0.018, VRerr=0.009, RIerr=0.012},
                new StandardStar() { Id = "27", Ra=132.913576, DEC=11.834474, V=12.247, BV=0.563, UB=0.067, VR=0.332, RI=0.325, Verr=0.013, BVerr=0.010, UBerr=0.015, VRerr=0.008, RIerr=0.013},
                new StandardStar() { Id = "28", Ra=132.838534, DEC=11.764721, V=12.253, BV=0.571, UB=0.072, VR=0.336, RI=0.320, Verr=0.014, BVerr=0.010, UBerr=0.017, VRerr=0.011, RIerr=0.009},
                new StandardStar() { Id = "29", Ra=132.785038, DEC=11.786758, V=12.380, BV=0.981, UB=0.765, VR=0.520, RI=0.452, Verr=0.014, BVerr=0.012, UBerr=0.025, VRerr=0.008, RIerr=0.015},
                new StandardStar() { Id = "30", Ra=132.819685, DEC=11.758237, V=12.391, BV=0.745, UB=0.270, VR=0.427, RI=0.394, Verr=0.012, BVerr=0.012, UBerr=0.017, VRerr=0.010, RIerr=0.014},
                new StandardStar() { Id = "31", Ra=132.927937, DEC=11.776905, V=12.411, BV=0.562, UB=-0.060, VR=0.342, RI=0.353, Verr=0.011, BVerr=0.010, UBerr=0.017, VRerr=0.008, RIerr=0.008},
                new StandardStar() { Id = "32", Ra=132.829347, DEC=11.834994, V=12.538, BV=0.587, UB=0.078, VR=0.349, RI=0.324, Verr=0.017, BVerr=0.010, UBerr=0.030, VRerr=0.009, RIerr=0.009},
                new StandardStar() { Id = "33", Ra=132.855825, DEC=11.792925, V=12.540, BV=0.590, UB=0.100, VR=0.347, RI=0.334, Verr=0.018, BVerr=0.010, UBerr=0.026, VRerr=0.009, RIerr=0.015},
                new StandardStar() { Id = "34", Ra=132.926998, DEC=11.831179, V=12.561, BV=0.579, UB=0.078, VR=0.344, RI=0.339, Verr=0.017, BVerr=0.011, UBerr=0.027, VRerr=0.012, RIerr=0.009},
                new StandardStar() { Id = "35", Ra=132.827969, DEC=11.784151, V=12.580, BV=0.781, UB=0.294, VR=0.462, RI=0.419, Verr=0.023, BVerr=0.011, UBerr=0.031, VRerr=0.010, RIerr=0.014},
                new StandardStar() { Id = "36", Ra=132.905977, DEC=11.834878, V=12.589, BV=0.581, UB=0.102, VR=0.348, RI=0.332, Verr=0.019, BVerr=0.009, UBerr=0.027, VRerr=0.009, RIerr=0.009},
                new StandardStar() { Id = "37", Ra=132.880267, DEC=11.764142, V=12.622, BV=0.569, UB=0.050, VR=0.335, RI=0.334, Verr=0.020, BVerr=0.011, UBerr=0.027, VRerr=0.007, RIerr=0.011},
                new StandardStar() { Id = "38", Ra=132.885303, DEC=11.797958, V=12.629, BV=0.613, UB=0.123, VR=0.357, RI=0.347, Verr=0.019, BVerr=0.011, UBerr=0.028, VRerr=0.009, RIerr=0.007},
                new StandardStar() { Id = "39", Ra=132.884072, DEC=11.834416, V=12.633, BV=0.582, UB=0.100, VR=0.343, RI=0.328, Verr=0.018, BVerr=0.011, UBerr=0.028, VRerr=0.008, RIerr=0.011},
                new StandardStar() { Id = "40", Ra=132.827365, DEC=11.822705, V=12.640, BV=0.606, UB=0.119, VR=0.355, RI=0.325, Verr=0.019, BVerr=0.012, UBerr=0.026, VRerr=0.010, RIerr=0.010},
                new StandardStar() { Id = "42", Ra=132.763671, DEC=11.763220, V=12.652, BV=0.612, UB=0.133, VR=0.353, RI=0.324, Verr=0.019, BVerr=0.013, UBerr=0.027, VRerr=0.010, RIerr=0.012},
                new StandardStar() { Id = "41", Ra=132.756562, DEC=11.826251, V=12.653, BV=0.618, UB=0.125, VR=0.355, RI=0.322, Verr=0.018, BVerr=0.009, UBerr=0.030, VRerr=0.008, RIerr=0.013},
                new StandardStar() { Id = "43", Ra=132.814463, DEC=11.792138, V=12.665, BV=0.502, UB=0.033, VR=0.298, RI=0.275, Verr=0.018, BVerr=0.011, UBerr=0.026, VRerr=0.009, RIerr=0.009},
                new StandardStar() { Id = "44", Ra=132.870125, DEC=11.866722, V=12.672, BV=0.667, UB=0.190, VR=0.387, RI=0.358, Verr=0.018, BVerr=0.010, UBerr=0.030, VRerr=0.010, RIerr=0.011},
                new StandardStar() { Id = "45", Ra=132.900157, DEC=11.776074, V=12.683, BV=0.694, UB=0.197, VR=0.396, RI=0.380, Verr=0.016, BVerr=0.009, UBerr=0.029, VRerr=0.009, RIerr=0.008},
                new StandardStar() { Id = "46", Ra=132.845584, DEC=11.813799, V=12.691, BV=0.558, UB=0.041, VR=0.330, RI=0.317, Verr=0.018, BVerr=0.010, UBerr=0.026, VRerr=0.007, RIerr=0.013},
                new StandardStar() { Id = "47", Ra=132.877514, DEC=11.820411, V=12.692, BV=0.588, UB=0.105, VR=0.348, RI=0.334, Verr=0.019, BVerr=0.010, UBerr=0.029, VRerr=0.012, RIerr=0.009},
                new StandardStar() { Id = "48", Ra=132.924889, DEC=11.727077, V=12.707, BV=0.571, UB=0.050, VR=0.329, RI=0.330, Verr=0.018, BVerr=0.011, UBerr=0.028, VRerr=0.012, RIerr=0.008},
                new StandardStar() { Id = "49", Ra=132.825073, DEC=11.765126, V=12.724, BV=0.560, UB=0.066, VR=0.332, RI=0.312, Verr=0.020, BVerr=0.010, UBerr=0.028, VRerr=0.012, RIerr=0.010},
                new StandardStar() { Id = "50", Ra=132.754518, DEC=11.836415, V=12.731, BV=0.737, UB=0.279, VR=0.420, RI=0.366, Verr=0.016, BVerr=0.009, UBerr=0.031, VRerr=0.008, RIerr=0.013},
                new StandardStar() { Id = "51", Ra=132.885121, DEC=11.800410, V=12.752, BV=0.818, UB=0.422, VR=0.450, RI=0.413, Verr=0.020, BVerr=0.011, UBerr=0.034, VRerr=0.010, RIerr=0.009},
                new StandardStar() { Id = "52", Ra=132.833954, DEC=11.778332, V=12.761, BV=0.554, UB=0.054, VR=0.327, RI=0.316, Verr=0.020, BVerr=0.010, UBerr=0.029, VRerr=0.008, RIerr=0.008},
                new StandardStar() { Id = "53", Ra=132.933480, DEC=11.773556, V=12.772, BV=0.742, UB=0.282, VR=0.408, RI=0.392, Verr=0.018, BVerr=0.011, UBerr=0.033, VRerr=0.007, RIerr=0.008},
                new StandardStar() { Id = "54", Ra=132.814043, DEC=11.837380, V=12.790, BV=0.560, UB=0.078, VR=0.331, RI=0.310, Verr=0.017, BVerr=0.010, UBerr=0.028, VRerr=0.009, RIerr=0.013},
                new StandardStar() { Id = "55", Ra=132.867390, DEC=11.824374, V=12.796, BV=0.461, UB=-0.020, VR=0.298, RI=0.301, Verr=0.044, BVerr=0.010, UBerr=0.029, VRerr=0.009, RIerr=0.011},
                new StandardStar() { Id = "56", Ra=132.789710, DEC=11.695902, V=12.811, BV=0.730, UB=0.308, VR=0.401, RI=0.378, Verr=0.023, BVerr=0.019, UBerr=0.043, VRerr=0.011, RIerr=0.014},
                new StandardStar() { Id = "57", Ra=132.850471, DEC=11.806161, V=12.815, BV=0.572, UB=0.067, VR=0.337, RI=0.322, Verr=0.018, BVerr=0.012, UBerr=0.028, VRerr=0.011, RIerr=0.007},
                new StandardStar() { Id = "58", Ra=132.868038, DEC=11.871597, V=12.819, BV=0.557, UB=0.071, VR=0.337, RI=0.320, Verr=0.018, BVerr=0.012, UBerr=0.031, VRerr=0.011, RIerr=0.013},
                new StandardStar() { Id = "59", Ra=132.811608, DEC=11.790066, V=12.821, BV=0.558, UB=0.071, VR=0.332, RI=0.302, Verr=0.019, BVerr=0.010, UBerr=0.028, VRerr=0.009, RIerr=0.009},
                new StandardStar() { Id = "60", Ra=132.892945, DEC=11.828924, V=12.854, BV=0.526, UB=0.038, VR=0.321, RI=0.311, Verr=0.019, BVerr=0.010, UBerr=0.027, VRerr=0.009, RIerr=0.014},
                new StandardStar() { Id = "61", Ra=132.835828, DEC=11.771293, V=12.891, BV=0.452, UB=0.004, VR=0.274, RI=0.263, Verr=0.018, BVerr=0.010, UBerr=0.027, VRerr=0.011, RIerr=0.016},
                new StandardStar() { Id = "62", Ra=132.872421, DEC=11.757749, V=12.906, BV=0.926, UB=0.627, VR=0.495, RI=0.451, Verr=0.018, BVerr=0.011, UBerr=0.038, VRerr=0.010, RIerr=0.013},
                new StandardStar() { Id = "63", Ra=132.936529, DEC=11.779532, V=12.958, BV=0.914, UB=0.592, VR=0.488, RI=0.455, Verr=0.019, BVerr=0.013, UBerr=0.040, VRerr=0.011, RIerr=0.010},
                new StandardStar() { Id = "64", Ra=132.815266, DEC=11.849008, V=12.986, BV=0.849, UB=0.482, VR=0.466, RI=0.411, Verr=0.019, BVerr=0.011, UBerr=0.033, VRerr=0.010, RIerr=0.009}};
        }
    }
}

