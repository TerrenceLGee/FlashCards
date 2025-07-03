namespace FlashCards.DataAccess.Interfaces;

public interface IDatabaseInitializer
{
    Task InitializeDatabaseAsync();
}