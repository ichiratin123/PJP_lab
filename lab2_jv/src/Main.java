import java.util.List;

public class Main {
    public static void main(String[] args) {
        String input = " -2 + (245 div 3);  // note\n2 mod 3 * hello";
        Lexer lexer = new Lexer(input);
        List<Token> tokens = lexer.tokenize();

        for (Token token : tokens) {
            System.out.println(token);
        }
    }
}
