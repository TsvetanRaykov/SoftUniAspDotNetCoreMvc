namespace Vxp.Web.ViewModels.Orders
{
    using Data.Models;
    using Services.Mapping;

    public class OrderProjectViewModel : IMapFrom<Project>, IMapTo<Project>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
