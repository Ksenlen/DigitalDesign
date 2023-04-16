--1) Сотрудника с максимальной заработной платой.
SELECT * 
FROM employee 
WHERE salary = (SELECT MAX(salary) 
                FROM employee);
             
--2) Вывести одно число: максимальную длину цепочки руководителей по таблице сотрудников (вычислить глубину дерева).
WITH RECURSIVE cte AS (
  SELECT Id, chief_id, 1 AS depth
  FROM employee
  WHERE chief_id IS NULL
  
  UNION ALL
 
  SELECT e.Id, e.chief_id, c.depth + 1
  FROM employee e
  JOIN cte c ON e.chief_id = c.Id
)
SELECT MAX(depth) FROM cte;
             
--3) Отдел, с максимальной суммарной зарплатой сотрудников. 
SELECT department_id, SUM(salary) AS total_salary
FROM employee
GROUP BY department_id
ORDER BY total_salary DESC
LIMIT 1;
          
--4) Сотрудника, чье имя начинается на «Р» и заканчивается на «н».
SELECT *
FROM Employee
WHERE name LIKE 'Р%н'        