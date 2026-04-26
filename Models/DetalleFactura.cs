namespace ClinicaAPI.Models
{
    public class DetalleFactura
    {
        public int Id { get; set; }
        public int FacturaId { get; set; } 
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; } 
    }
}
