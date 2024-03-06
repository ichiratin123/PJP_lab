from enum import Enum
import re

class Token:
    class Type(Enum):
        NUM = "NUM"
        OP = "OP"
        LPAR = "LPAR"
        RPAR = "RPAR"
        SEMICOLON = "SEMICOLON"
        DIV = "DIV"
        MOD = "MOD"
        ID = "ID"
        EOF = "EOF"

    def __init__(self, type, value):
        self.type = type
        self.value = value

    def get_type(self):
        return self.type

    def get_value(self):
        return self.value

    def __str__(self):
        if self.type in [Token.Type.LPAR, Token.Type.RPAR, Token.Type.SEMICOLON, Token.Type.DIV, Token.Type.MOD]:
            return self.type.value
        else:
            return f"{self.type.value}:{self.value if self.value is not None else ''}"

class Lexer:
    def __init__(self, filename):
        self.tokens = []
        self.buffer = []
        with open(filename, 'r') as file:
            self.lines = file.readlines()

    def is_operator(self, c):
        return c in ['+', '-', '*']

    def is_parenthesis(self, c):
        return c in ['(', ')']

    def add_buffer_as_token(self):
        if self.buffer:
            str_buffer = ''.join(self.buffer)
            if re.match(r'\d+', str_buffer):
                self.tokens.append(Token(Token.Type.NUM, str_buffer))
            elif re.match(r'[a-zA-Z]+', str_buffer):
                if str_buffer.lower() == "div":
                    self.tokens.append(Token(Token.Type.DIV, str_buffer))
                elif str_buffer.lower() == "mod":
                    self.tokens.append(Token(Token.Type.MOD, str_buffer))
                else:
                    self.tokens.append(Token(Token.Type.ID, str_buffer))
            self.buffer = []

    def tokenize(self):
        for line in self.lines:
            line = line.split('//')[0].strip()
            for c in line:
                if c.isspace():
                    self.add_buffer_as_token()
                elif self.is_operator(c):
                    self.add_buffer_as_token()
                    self.tokens.append(Token(Token.Type.OP, c))
                elif self.is_parenthesis(c):
                    self.add_buffer_as_token()
                    if c == '(':
                        self.tokens.append(Token(Token.Type.LPAR, ""))
                    else:
                        self.tokens.append(Token(Token.Type.RPAR, ""))
                elif c == ';':
                    self.add_buffer_as_token()
                    self.tokens.append(Token(Token.Type.SEMICOLON, ""))
                else:
                    self.buffer.append(c)
            self.add_buffer_as_token()

    def get_tokens(self):
        return self.tokens
    
file_path = "input.txt"

try:
    lexer = Lexer(file_path)
    lexer.tokenize()
    tokens = lexer.get_tokens()

    for token in tokens:
        print(token)
except IOError as e:
    print("An error occurred while reading the file.")
    print(e)
