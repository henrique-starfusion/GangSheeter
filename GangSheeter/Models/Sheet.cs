using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GangSheeter.Models;

public class Sheet
{
    public List<ImageSheet> Images { get; } = new List<ImageSheet>();

    /// <summary>
    /// Largura em centimetros
    /// </summary>
    public decimal Width { get; set; } = 58;

    /// <summary>
    /// Altura em centimetros
    /// </summary>
    public decimal Height { get; set; } = 100;

    /// <summary>
    /// Resolução em DPI
    /// </summary>
    public int Resolution { get; set; }

    public void AddImages(IList<ImageSheet> images) => Images.AddRange(images);
}
