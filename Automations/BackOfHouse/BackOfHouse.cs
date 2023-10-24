using ChandlerHome.Automations;

namespace ChandlerHome.Automations.BackOfHouse;

[NetDaemonApp(Id = "Back Of House Base")]
internal class BackOfHouse : Home
{
    private bool notificationSent;
    public BackOfHouse(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.Cover.BasementGarageDoor.StateAllChanges().WhenStateIsFor(x => x.IsOff(), TimeSpan.FromHours(1))
        .Subscribe(x =>
            {
                if (!notificationSent)
                {
                    var services = new Services(ha);

                    services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters
                    {
                        Title = $"Garage has been open for over 1 hour.",
                        Message = "Shut it if you forgot, otherwise ignore me."
                    });

                    notificationSent = true;
                }
            });

        _entities.Cover.BasementGarageDoor.StateChanges().Where(x => x.New.IsOn())
            .Subscribe(x =>
            {
                notificationSent = false;
            });
    }
}
