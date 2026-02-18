using System;
using DesignPatternChallenge.Adapters;
using DesignPatternChallenge.Legacy;
using DesignPatternChallenge.Processors;
using DesignPatternChallenge.Services;

namespace DesignPatternChallenge;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Sistema de Checkout ===\n");

        // 1. Checkout com processador moderno (funciona normalmente)
        var modernProcessor = new ModernPaymentProcessor();
        var checkoutWithModern = new CheckoutService(modernProcessor);
        checkoutWithModern.CompleteOrder("cliente@email.com", 150.00m, "4111111111111111");

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // 2. Checkout com sistema legado via Adapter (mesma interface!)
        var legacySystem = new LegacyPaymentSystem();
        var legacyAdapter = new LegacyPaymentAdapter(legacySystem);
        var checkoutWithLegacy = new CheckoutService(legacyAdapter);
        checkoutWithLegacy.CompleteOrder("cliente2@email.com", 200.00m, "4111111111111111");

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // 3. Demonstrando que ambos coexistem de forma transparente
        Console.WriteLine("✅ Ambos os processadores funcionam com a mesma interface!");
        Console.WriteLine("   - CheckoutService não foi modificado");
        Console.WriteLine("   - Sistema legado não foi modificado");
        Console.WriteLine("   - O Adapter encapsula todas as conversões");
    }
}
