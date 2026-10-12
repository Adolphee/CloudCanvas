import azure.functions as func
from functions.generate_tags_function import blueprint as tagging_function
from functions.generate_caption_function import blueprint as caption_function

app = func.FunctionApp()
app.register_functions(tagging_function)
app.register_functions(caption_function)