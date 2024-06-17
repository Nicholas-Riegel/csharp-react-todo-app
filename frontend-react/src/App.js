import { useState, useEffect } from 'react';
import * as todoService from './services/todoService';

const App = () => {

const initialTodo = {todo_text: ''};
const [todoArray, setTodoArray] = useState([]);
const [todo, setTodo] = useState(initialTodo);

const fetchTodos = async () => {
  const data = await todoService.getAllTodos();
  setTodoArray(data);
};

	const handleAddTodo = async (data) => {
		try {
			const newTodo = await todoService.createTodo(data);
			setTodoArray([newTodo, ...todoArray]);
			// setIsFormOpen(false);
		} catch (error) {
			console.log(error);
		}
	};

	const handleUpdateTodo = async (data, todoId) => {
		try {
			const updatedTodo = await todoService.updateTodo(data, todoId);
	
			if (updatedTodo.error) {
				throw new Error(updatedTodo.error);
			}
	
			const updatedTodoArray = todoArray.map((todo) =>
				todo.id !== updatedTodo.id ? todo : updatedTodo
			);
			setTodoArray(updatedTodoArray);
		} catch (error) {
			console.log(error);
		}
	};
  
	const handleRemoveTodo = async (todoId) => {
		try {
			const deletedTodo = await todoService.deleteTodo(todoId);
	
			if (deletedTodo.error) {
				throw new Error(deletedTodo.error);
			}
	
			const updatedTodoArray = todoArray.filter((todo) =>
				todo.id !== todoId
			);
			setTodoArray(updatedTodoArray);
		} catch (error) {
			console.log(error);
		}
	}
  useEffect(() => {

		fetchTodos();
	
	}, []);
	return (
		<>
    <h1>Todos</h1>
    {todoArray.map((todo) => (
      <div key={todo.id}>
        <p>{todo.todo_text}</p>
        <button onClick={() => handleUpdateTodo(todo)}>Edit</button>
        <button onClick={() => handleRemoveTodo(todo.id)}>Delete</button>
      </div>
    ))}
    <input 
      type="text" 
      placeholder="Add a todo"
      value={todo.todo_text}
      onChange={} />
    <button onClick={() => handleAddTodo()}>Add</button>
		</>
	)
}

export default App;