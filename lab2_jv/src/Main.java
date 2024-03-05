import java.io.IOException;
import java.util.List;

public class Main {
    public static void main(String[] args) {
        // Assuming 'input.txt' is in the current directory, otherwise specify the full path
        String filePath = "E:\\code\\PJP\\PJP_lab\\lab2_jv\\src\\input.txt";

        try {
            Lexer lexer = new Lexer(filePath);
            lexer.tokenize();
            List<Token> tokens = lexer.getTokens();

            // Print out the tokens
            for (Token token : tokens) {
                System.out.println(token);
            }
        } catch (IOException e) {
            System.out.println("An error occurred while reading the file.");
            e.printStackTrace();
        }
    }
}
