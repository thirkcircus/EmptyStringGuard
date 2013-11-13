using System;
using System.Threading.Tasks;
using EmptyStringGuard;

public class SpecialClass
{
#if (DEBUG)
    public async Task SomeMethodAsync(string nonEmptyArg, [AllowEmpty] string emptyArg) {
        await Task.Run(() => Console.WriteLine(nonEmptyArg));
    }
#endif
}