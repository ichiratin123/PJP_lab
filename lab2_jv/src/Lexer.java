import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class Lexer {
    private final Scanner scanner;
    private List<Token> tokens;
    private StringBuilder buffer;

    public Lexer(String filename) throws IOException {
        scanner = new Scanner(new FileReader(filename));
        tokens = new ArrayList<>();
        buffer = new StringBuilder();
    }

    private boolean isOperator(char c) {
        return c == '+' || c == '-' || c == '*';
    }

    private boolean isParenthesis(char c) {
        return c == '(' || c == ')';
    }

    private void addBufferAsToken() {
        if (buffer.length() > 0) {
            String str = buffer.toString();
            if (str.matches("\\d+")) {
                tokens.add(new Token(Token.Type.NUM, str));
            } else if (str.matches("[a-zA-Z]+")) {
                if (str.equalsIgnoreCase("div")) {
                    tokens.add(new Token(Token.Type.DIV, str));
                } else if (str.equalsIgnoreCase("mod")) {
                    tokens.add(new Token(Token.Type.MOD, str));
                } else {
                    tokens.add(new Token(Token.Type.ID, str));
                }
            }
            buffer = new StringBuilder();
        }
    }

    public void tokenize() {
        while (scanner.hasNextLine()) {
            String line = scanner.nextLine();
            if (line.contains("//")) {
                line = line.substring(0, line.indexOf("//")).trim();
            }
            for (int i = 0; i < line.length(); i++) {
                char c = line.charAt(i);
                if (Character.isWhitespace(c)) {
                    addBufferAsToken();
                } else if (isOperator(c)) {
                    addBufferAsToken();
                    tokens.add(new Token(Token.Type.OP, String.valueOf(c)));
                } else if (isParenthesis(c)) {
                    addBufferAsToken();
                    if (c == '(') {
                        tokens.add(new Token(Token.Type.LPAR, ""));
                    } else {
                        tokens.add(new Token(Token.Type.RPAR, ""));
                    }
                } else if (c == ';') {
                    addBufferAsToken();
                    tokens.add(new Token(Token.Type.SEMICOLON, ""));
                } else {
                    buffer.append(c);
                }
            }
            addBufferAsToken();
        }
        scanner.close();
    }

    public List<Token> getTokens() {
        return tokens;
    }
}
