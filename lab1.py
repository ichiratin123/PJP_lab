def priorityOp(op):
    if op in ('+', '-'):
        return 1
    if op in ('*', '/'):
        return 2
    return 0

def convertExp(exp):
    def compareOp(i):
        try:
            a = priorityOp(i)
            b = priorityOp(stack[-1])
            return a <= b
        except IndexError:
            return False
    
    exp = exp[::-1]
    exp = exp.replace('(', 'temp')
    exp = exp.replace(')', '(')
    exp = exp.replace('temp', ')')

    stack = []
    result = []
    
    for char in exp:
        if char.isalnum():
            result.append(char)
        elif char == '(':
            stack.append(char)
        elif char == ')':
            while stack and stack[-1] != '(':
                result.append(stack.pop())
            stack.pop() # Pop '('
        else:
            while (stack and compareOp(char)):
                result.append(stack.pop())
            stack.append(char)
    while stack:
        result.append(stack.pop())
    
    return result

def removeSpace(s):
    s = s.replace(" ", "")
    return s

def cal(exp):
    stack = []
    for i in exp:
        if i.isdigit():
            stack.append(int(i))
        else:
            op1 = stack.pop()
            op2 = stack.pop()
            
            if i == "+":
                stack.append(op1 + op2)
            elif i == "-":
                stack.append(op1 - op2)
            elif i == "*":
                stack.append(op1 * op2)
            elif i == "/":
                stack.append(int(op1 / op2)) 
    print(stack)

s = "2 * (3+5)"
try:
    cal(convertExp(removeSpace(s)))
except:
    print("Error")