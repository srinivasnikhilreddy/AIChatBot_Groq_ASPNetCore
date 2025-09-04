
document.addEventListener("DOMContentLoaded", () => {
    const chatContainer = document.getElementById("chatContainer");
    const userInput = document.getElementById("userInput");
    const submitBtn = document.getElementById("submitBtn");

    function addMessage(text, sender)
    {
        const msg = document.createElement("div");
        msg.classList.add("message", sender === "user" ? "user-message" : "ai-message");
        msg.innerHTML = text;
        chatContainer.appendChild(msg);
        chatContainer.scrollTop = chatContainer.scrollHeight;
        return msg;
    }

    submitBtn.addEventListener("click", async () => {
        const input = userInput.value.trim();
        if(!input) return;

        addMessage(input, "user");
        userInput.value = "";

        const aiResponse = addMessage("<i>Thinking...</i>", "ai");

        try{
            const response = await fetch("/Groq/chat", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(input)
            });

            if (!response.ok){
                aiResponse.innerHTML = "❌ Error: " + response.status;
                return;
            }

            const reader = response.body.getReader();
            const decoder = new TextDecoder();
            let fullText = "";

            while(true)
            {
                const { done, value } = await reader.read();
                if(done) break;

                fullText += decoder.decode(value, { stream: true });
                aiResponse.innerHTML = marked.parse(fullText);
                chatContainer.scrollTop = chatContainer.scrollHeight;
            }
        }catch(e){
            aiResponse.innerHTML = "Request failed: " + e.message;
        }
    });
});
